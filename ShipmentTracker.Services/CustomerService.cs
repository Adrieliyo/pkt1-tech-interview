using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Customers;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;
using System.Linq.Expressions;

namespace ShipmentTracker.Services
{
    public class CustomerService : ICustomerService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateIndividualCustomerDto> _createIndividualValidator;
        private readonly IValidator<CreateBusinessCustomerDto> _createBusinessValidator;
        private readonly IValidator<UpdateCustomerDto> _updateValidator;

        public CustomerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateIndividualCustomerDto> createIndividualValidator,
            IValidator<CreateBusinessCustomerDto> createBusinessValidator,
            IValidator<UpdateCustomerDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createIndividualValidator = createIndividualValidator;
            _createBusinessValidator = createBusinessValidator;
            _updateValidator = updateValidator;
        }

        // Unicidad de Email a nivel compañía, contra todos los clientes (ambos tipos, activos e
        // inactivos, research.md Decisión 7).
        private async Task<List<ValidationFailure>> ValidateEmailUniquenessAsync(string email, int currentId)
        {
            var failures = new List<ValidationFailure>();

            var emailInUse = await _unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.Email == email && x.Id != currentId);
            if (emailInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(Customer.Email), "El correo electrónico ya está en uso por otro cliente."));
            }

            return failures;
        }

        // Unicidad de GovernmentId solo entre clientes Individual (activos e inactivos).
        private async Task<List<ValidationFailure>> ValidateGovernmentIdUniquenessAsync(string governmentId, int currentId)
        {
            var failures = new List<ValidationFailure>();

            Expression<Func<Customer, bool>> filter = x => x is IndividualCustomer && ((IndividualCustomer)x).GovernmentId == governmentId && x.Id != currentId;
            var governmentIdInUse = await _unitOfWork.CustomerRepository.SingleOrDefaultAsync(filter);
            if (governmentIdInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(IndividualCustomer.GovernmentId), "El identificador gubernamental ya está en uso por otro cliente."));
            }

            return failures;
        }

        // Unicidad de TaxId solo entre clientes Business (activos e inactivos).
        private async Task<List<ValidationFailure>> ValidateTaxIdUniquenessAsync(string taxId, int currentId)
        {
            var failures = new List<ValidationFailure>();

            Expression<Func<Customer, bool>> filter = x => x is BusinessCustomer && ((BusinessCustomer)x).TaxId == taxId && x.Id != currentId;
            var taxIdInUse = await _unitOfWork.CustomerRepository.SingleOrDefaultAsync(filter);
            if (taxIdInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(BusinessCustomer.TaxId), "El RFC ya está en uso por otro cliente."));
            }

            return failures;
        }

        public async Task<CustomerDetailDto> CreateIndividualAsync(CreateIndividualCustomerDto dto)
        {
            dto.Email = dto.Email?.Trim() ?? string.Empty;
            dto.GovernmentId = dto.GovernmentId?.Trim() ?? string.Empty;
            dto.FirstName = dto.FirstName?.Trim() ?? string.Empty;
            dto.LastName = dto.LastName?.Trim() ?? string.Empty;

            var structuralResult = await _createIndividualValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateEmailUniquenessAsync(dto.Email, currentId: 0));
            failures.AddRange(await ValidateGovernmentIdUniquenessAsync(dto.GovernmentId, currentId: 0));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var customer = new IndividualCustomer
            {
                Type = CustomerType.Individual,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                GovernmentId = dto.GovernmentId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CustomerDetailDto>(customer);
        }

        public async Task<CustomerDetailDto> CreateBusinessAsync(CreateBusinessCustomerDto dto)
        {
            dto.Email = dto.Email?.Trim() ?? string.Empty;
            dto.TaxId = dto.TaxId?.Trim() ?? string.Empty;
            dto.BusinessName = dto.BusinessName?.Trim() ?? string.Empty;
            dto.LegalRepresentative = dto.LegalRepresentative?.Trim() ?? string.Empty;

            var structuralResult = await _createBusinessValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateEmailUniquenessAsync(dto.Email, currentId: 0));
            failures.AddRange(await ValidateTaxIdUniquenessAsync(dto.TaxId, currentId: 0));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var customer = new BusinessCustomer
            {
                Type = CustomerType.Business,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country,
                BusinessName = dto.BusinessName,
                TaxId = dto.TaxId,
                LegalRepresentative = dto.LegalRepresentative,
                Industry = dto.Industry,
                CreditLimit = dto.CreditLimit,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CustomerDetailDto>(customer);
        }

        public async Task<PagedResult<CustomerDetailDto>> GetCustomersAsync(bool onlyActive = true, CustomerType? type = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Customer, bool>> filter = type.HasValue
                ? x => x.IsActive == onlyActive && x.Type == type.Value
                : x => x.IsActive == onlyActive;

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Customer> customers;
            if (skip > int.MaxValue)
            {
                customers = Enumerable.Empty<Customer>();
            }
            else
            {
                customers = await _unitOfWork.CustomerRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.CustomerRepository.CountAsync(filter);

            return new PagedResult<CustomerDetailDto>
            {
                Items = customers.Select(c => _mapper.Map<CustomerDetailDto>(c)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CustomerDetailDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (customer == null)
                return null;

            return _mapper.Map<CustomerDetailDto>(customer);
        }

        public async Task<CustomerDetailDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (customer == null)
                return null;

            dto.Email = dto.Email?.Trim() ?? string.Empty;
            dto.FirstName = dto.FirstName?.Trim();
            dto.LastName = dto.LastName?.Trim();
            dto.GovernmentId = dto.GovernmentId?.Trim();
            dto.BusinessName = dto.BusinessName?.Trim();
            dto.TaxId = dto.TaxId?.Trim();
            dto.LegalRepresentative = dto.LegalRepresentative?.Trim();

            var structuralResult = await _updateValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateEmailUniquenessAsync(dto.Email, currentId: id));

            // El tipo real ya persistido decide qué campos son requeridos/prohibidos — no hay
            // campo Type en el DTO, así que un cambio de tipo es imposible por construcción
            // (FR-004). Cross-type field rejection (FR-013) y completitud (FR-005) se resuelven
            // aquí, no en el validador estructural (research.md Decisión 8).
            if (customer is IndividualCustomer individual)
            {
                if (dto.BusinessName != null || dto.TaxId != null || dto.LegalRepresentative != null || dto.Industry != null || dto.CreditLimit.HasValue)
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.BusinessName), "No se pueden enviar campos de Business para un cliente Individual."));
                }

                if (string.IsNullOrEmpty(dto.FirstName))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.FirstName), "El nombre es requerido."));
                }

                if (string.IsNullOrEmpty(dto.LastName))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.LastName), "El apellido es requerido."));
                }

                if (string.IsNullOrEmpty(dto.GovernmentId))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.GovernmentId), "El identificador gubernamental (CURP) es requerido."));
                }
                else
                {
                    failures.AddRange(await ValidateGovernmentIdUniquenessAsync(dto.GovernmentId, currentId: id));
                }

                if (!failures.Any())
                {
                    individual.FirstName = dto.FirstName!;
                    individual.LastName = dto.LastName!;
                    individual.BirthDate = dto.BirthDate;
                    individual.GovernmentId = dto.GovernmentId!;
                }
            }
            else if (customer is BusinessCustomer business)
            {
                if (dto.FirstName != null || dto.LastName != null || dto.BirthDate.HasValue || dto.GovernmentId != null)
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.FirstName), "No se pueden enviar campos de Individual para un cliente Business."));
                }

                if (string.IsNullOrEmpty(dto.BusinessName))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.BusinessName), "La razón social es requerida."));
                }

                if (string.IsNullOrEmpty(dto.LegalRepresentative))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.LegalRepresentative), "El representante legal es requerido."));
                }

                if (string.IsNullOrEmpty(dto.TaxId))
                {
                    failures.Add(new ValidationFailure(nameof(UpdateCustomerDto.TaxId), "El RFC es requerido."));
                }
                else
                {
                    failures.AddRange(await ValidateTaxIdUniquenessAsync(dto.TaxId, currentId: id));
                }

                if (!failures.Any())
                {
                    business.BusinessName = dto.BusinessName!;
                    business.TaxId = dto.TaxId!;
                    business.LegalRepresentative = dto.LegalRepresentative!;
                    business.Industry = dto.Industry;
                    business.CreditLimit = dto.CreditLimit;
                }
            }

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;
            customer.City = dto.City;
            customer.State = dto.State;
            customer.ZipCode = dto.ZipCode;
            customer.Country = dto.Country;
            customer.IsActive = dto.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CustomerDetailDto>(customer);
        }

        public async Task<bool> DeactivateCustomerAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (customer == null)
                return false;

            if (customer.IsActive)
            {
                customer.IsActive = false;
                await _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.CommitAsync();
            }

            return true;
        }
    }
}
