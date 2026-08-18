using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Infrastructure.Data.Configurations;
using ShipmentTracker.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Shipment> Shipments { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<BranchSchedule> BranchSchedules { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<IndividualCustomer> IndividualCustomers { get; set; }

        public DbSet<BusinessCustomer> BusinessCustomers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ShipmentEvent> ShipmentEvents { get; set; }

        public DbSet<DeliveryAttempt> DeliveryAttempts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new ShipmentConfiguration());
            builder.ApplyConfiguration(new BranchConfiguration());
            builder.ApplyConfiguration(new BranchScheduleConfiguration());
            builder.ApplyConfiguration(new EmployeeConfiguration());
            builder.ApplyConfiguration(new VehicleConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new IndividualCustomerConfiguration());
            builder.ApplyConfiguration(new BusinessCustomerConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new ShipmentEventConfiguration());
            builder.ApplyConfiguration(new DeliveryAttemptConfiguration());
        }
    }
}
