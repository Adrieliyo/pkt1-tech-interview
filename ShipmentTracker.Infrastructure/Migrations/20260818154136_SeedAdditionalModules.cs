using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Seedea 5 registros para cada uno de los módulos que ya existen para cuando esta migración
    /// corre (Branches, Employees, Vehicles, Customers, Orders, Shipments, ShipmentEvents). Se crea
    /// como migración NUEVA — no se edita <c>SeedInitialData</c> — porque esa migración corre antes
    /// de que existan las tablas de estos módulos (fechada 2026-07-28, antes de los módulos 003-008);
    /// insertar en ellas ahí rompería un `dotnet ef database update` sobre una base de datos nueva
    /// con "Invalid object name".
    /// Los IDs auto-generados (IDENTITY) se capturan con SCOPE_IDENTITY() y se encadenan como FKs
    /// en un único batch T-SQL, en orden de dependencia: Branches → Employees/Vehicles → Customers
    /// (TPT) → Orders → Shipments → ShipmentEvents.
    /// </remarks>
    public partial class SeedAdditionalModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
DECLARE @b1 INT, @b2 INT, @b3 INT, @b4 INT, @b5 INT;

-- ===== Branches (5) =====
INSERT INTO Branches (Name, Type, Address, City, State, ZipCode, Latitude, Longitude, Phone, IsActive, CreatedAt)
VALUES (N'Sucursal Centro', 'Hub', N'Av. Reforma 100', N'Ciudad de Mexico', N'CDMX', '06000', 19.4326, -99.1332, '+52-55-1000-0001', 1, '2026-08-01T08:00:00');
SET @b1 = SCOPE_IDENTITY();

INSERT INTO Branches (Name, Type, Address, City, State, ZipCode, Latitude, Longitude, Phone, IsActive, CreatedAt)
VALUES (N'Sucursal Norte', 'Hub', N'Av. Constitucion 500', N'Monterrey', N'Nuevo Leon', '64000', 25.6866, -100.3161, '+52-81-1000-0002', 1, '2026-08-01T08:15:00');
SET @b2 = SCOPE_IDENTITY();

INSERT INTO Branches (Name, Type, Address, City, State, ZipCode, Latitude, Longitude, Phone, IsActive, CreatedAt)
VALUES (N'Sucursal Sur', 'SalesPoint', N'Av. Vallarta 200', N'Guadalajara', N'Jalisco', '44100', 20.6597, -103.3496, '+52-33-1000-0003', 1, '2026-08-01T08:30:00');
SET @b3 = SCOPE_IDENTITY();

INSERT INTO Branches (Name, Type, Address, City, State, ZipCode, Latitude, Longitude, Phone, IsActive, CreatedAt)
VALUES (N'Punto de Recoleccion Poniente', 'PickupPoint', N'Calle Poniente 45', N'Ciudad de Mexico', N'CDMX', '01000', NULL, NULL, NULL, 1, '2026-08-01T08:45:00');
SET @b4 = SCOPE_IDENTITY();

INSERT INTO Branches (Name, Type, Address, City, State, ZipCode, Latitude, Longitude, Phone, IsActive, CreatedAt)
VALUES (N'Oficina Corporativa', 'Headquarters', N'Paseo de la Reforma 350', N'Ciudad de Mexico', N'CDMX', '06600', NULL, NULL, '+52-55-1000-0005', 1, '2026-08-01T09:00:00');
SET @b5 = SCOPE_IDENTITY();

DECLARE @e1 INT, @e2 INT, @e3 INT, @e4 INT, @e5 INT;

-- ===== Employees (5) =====
INSERT INTO Employees (BranchId, FirstName, LastName, Email, Phone, Role, EmployeeNumber, HireDate, IsActive, CreatedAt, UpdatedAt)
VALUES (@b1, N'Ana', N'Garcia', 'ana.garcia.seed@example.com', '+52-55-2000-0001', 'BranchManager', 'SEED-E0001', '2026-01-15', 1, '2026-08-01T09:00:00', NULL);
SET @e1 = SCOPE_IDENTITY();

INSERT INTO Employees (BranchId, FirstName, LastName, Email, Phone, Role, EmployeeNumber, HireDate, IsActive, CreatedAt, UpdatedAt)
VALUES (@b1, N'Carlos', N'Ruiz', 'carlos.ruiz.seed@example.com', '+52-55-2000-0002', 'Operator', 'SEED-E0002', '2026-02-01', 1, '2026-08-01T09:05:00', NULL);
SET @e2 = SCOPE_IDENTITY();

INSERT INTO Employees (BranchId, FirstName, LastName, Email, Phone, Role, EmployeeNumber, HireDate, IsActive, CreatedAt, UpdatedAt)
VALUES (@b2, N'Maria', N'Lopez', 'maria.lopez.seed@example.com', '+52-81-2000-0003', 'Driver', 'SEED-E0003', '2026-02-10', 1, '2026-08-01T09:10:00', NULL);
SET @e3 = SCOPE_IDENTITY();

INSERT INTO Employees (BranchId, FirstName, LastName, Email, Phone, Role, EmployeeNumber, HireDate, IsActive, CreatedAt, UpdatedAt)
VALUES (@b2, N'Jorge', N'Diaz', 'jorge.diaz.seed@example.com', '+52-81-2000-0004', 'WarehouseStaff', 'SEED-E0004', '2026-03-01', 1, '2026-08-01T09:15:00', NULL);
SET @e4 = SCOPE_IDENTITY();

INSERT INTO Employees (BranchId, FirstName, LastName, Email, Phone, Role, EmployeeNumber, HireDate, IsActive, CreatedAt, UpdatedAt)
VALUES (@b3, N'Sofia', N'Torres', 'sofia.torres.seed@example.com', '+52-33-2000-0005', 'Driver', 'SEED-E0005', '2026-03-15', 1, '2026-08-01T09:20:00', NULL);
SET @e5 = SCOPE_IDENTITY();

-- ===== Vehicles (5) =====
INSERT INTO Vehicles (BranchId, Plate, Type, Brand, Model, Year, MaxWeightKg, IsActive, CreatedAt)
VALUES (@b1, 'SEED-001', 'Motorcycle', N'Honda', N'CG150', 2023, 50.00, 1, '2026-08-01T09:30:00');

INSERT INTO Vehicles (BranchId, Plate, Type, Brand, Model, Year, MaxWeightKg, IsActive, CreatedAt)
VALUES (@b2, 'SEED-002', 'Van', N'Ford', N'Transit', 2022, 1200.00, 1, '2026-08-01T09:35:00');

INSERT INTO Vehicles (BranchId, Plate, Type, Brand, Model, Year, MaxWeightKg, IsActive, CreatedAt)
VALUES (@b2, 'SEED-003', 'Truck', N'Freightliner', N'M2', 2021, 8000.00, 1, '2026-08-01T09:40:00');

INSERT INTO Vehicles (BranchId, Plate, Type, Brand, Model, Year, MaxWeightKg, IsActive, CreatedAt)
VALUES (@b3, 'SEED-004', 'Van', N'Nissan', N'NV200', 2023, 800.00, 1, '2026-08-01T09:45:00');

INSERT INTO Vehicles (BranchId, Plate, Type, Brand, Model, Year, MaxWeightKg, IsActive, CreatedAt)
VALUES (@b1, 'SEED-005', 'Motorcycle', N'Yamaha', N'FZ150', 2024, 45.00, 1, '2026-08-01T09:50:00');

DECLARE @c1 INT, @c2 INT, @c3 INT, @c4 INT, @c5 INT;

-- ===== Customers (5: 3 Individual + 2 Business, TPT) =====
INSERT INTO Customers (Type, Email, Phone, Address, City, State, ZipCode, Country, IsActive, CreatedAt, UpdatedAt)
VALUES ('Individual', 'laura.hernandez.seed@example.com', '+52-55-3000-0001', N'Calle Insurgentes 12', N'Ciudad de Mexico', N'CDMX', '03100', N'Mexico', 1, '2026-08-01T10:00:00', NULL);
SET @c1 = SCOPE_IDENTITY();
INSERT INTO IndividualCustomers (Id, FirstName, LastName, BirthDate, GovernmentId)
VALUES (@c1, N'Laura', N'Hernandez', '1990-05-12', 'SEEDGOVID0001');

INSERT INTO Customers (Type, Email, Phone, Address, City, State, ZipCode, Country, IsActive, CreatedAt, UpdatedAt)
VALUES ('Individual', 'miguel.sanchez.seed@example.com', '+52-55-3000-0002', N'Calle Madero 45', N'Ciudad de Mexico', N'CDMX', '06000', N'Mexico', 1, '2026-08-01T10:05:00', NULL);
SET @c2 = SCOPE_IDENTITY();
INSERT INTO IndividualCustomers (Id, FirstName, LastName, BirthDate, GovernmentId)
VALUES (@c2, N'Miguel', N'Sanchez', '1985-11-03', 'SEEDGOVID0002');

INSERT INTO Customers (Type, Email, Phone, Address, City, State, ZipCode, Country, IsActive, CreatedAt, UpdatedAt)
VALUES ('Individual', 'patricia.flores.seed@example.com', '+52-33-3000-0003', N'Av. Chapultepec 89', N'Guadalajara', N'Jalisco', '44100', N'Mexico', 1, '2026-08-01T10:10:00', NULL);
SET @c3 = SCOPE_IDENTITY();
INSERT INTO IndividualCustomers (Id, FirstName, LastName, BirthDate, GovernmentId)
VALUES (@c3, N'Patricia', N'Flores', NULL, 'SEEDGOVID0003');

INSERT INTO Customers (Type, Email, Phone, Address, City, State, ZipCode, Country, IsActive, CreatedAt, UpdatedAt)
VALUES ('Business', 'contacto.comercializadoraxyz.seed@example.com', '+52-55-3000-0004', N'Av. Industria 300', N'Ciudad de Mexico', N'CDMX', '02300', N'Mexico', 1, '2026-08-01T10:15:00', NULL);
SET @c4 = SCOPE_IDENTITY();
INSERT INTO BusinessCustomers (Id, BusinessName, TaxId, LegalRepresentative, Industry, CreditLimit)
VALUES (@c4, N'Comercializadora XYZ SA de CV', 'CXY990101AB1', N'Roberto Jimenez', N'Retail', 50000.00);

INSERT INTO Customers (Type, Email, Phone, Address, City, State, ZipCode, Country, IsActive, CreatedAt, UpdatedAt)
VALUES ('Business', 'contacto.distribuidoraabc.seed@example.com', '+52-81-3000-0005', N'Blvd. Industrial 77', N'Monterrey', N'Nuevo Leon', '64100', N'Mexico', 1, '2026-08-01T10:20:00', NULL);
SET @c5 = SCOPE_IDENTITY();
INSERT INTO BusinessCustomers (Id, BusinessName, TaxId, LegalRepresentative, Industry, CreditLimit)
VALUES (@c5, N'Distribuidora ABC SA de CV', 'DAB980202CD2', N'Fernanda Ortega', N'Logistica', 120000.00);

DECLARE @o1 INT, @o2 INT, @o3 INT, @o4 INT, @o5 INT;

-- ===== Orders (5) =====
INSERT INTO Orders (OrderNumber, CustomerId, OriginBranchId, Status, ServiceType, PickupType, PickupAddress, PickupScheduledAt, RecipientName, RecipientPhone, RecipientAddress, RecipientCity, RecipientState, RecipientZipCode, DeclaredWeightKg, DeclaredWidthCm, DeclaredHeightCm, DeclaredLengthCm, QuotedPrice, Notes, CreatedAt, UpdatedAt)
VALUES ('ORD-SEED0001', @c1, NULL, 'Pending', 'Standard', 'HomePickup', N'Calle Insurgentes 12', '2026-08-05T10:00:00', N'Pedro Ramirez', '+52-55-4000-0001', N'Calle Juarez 22', N'Puebla', N'Puebla', '72000', 2.500, 20.00, 15.00, 30.00, 150.00, N'Entrega en horario de oficina.', '2026-08-01T11:00:00', NULL);
SET @o1 = SCOPE_IDENTITY();

INSERT INTO Orders (OrderNumber, CustomerId, OriginBranchId, Status, ServiceType, PickupType, PickupAddress, PickupScheduledAt, RecipientName, RecipientPhone, RecipientAddress, RecipientCity, RecipientState, RecipientZipCode, DeclaredWeightKg, DeclaredWidthCm, DeclaredHeightCm, DeclaredLengthCm, QuotedPrice, Notes, CreatedAt, UpdatedAt)
VALUES ('ORD-SEED0002', @c2, @b1, 'Confirmed', 'Express', 'DropOff', NULL, NULL, N'Lucia Mendoza', '+52-55-4000-0002', N'Av. Universidad 500', N'Ciudad de Mexico', N'CDMX', '04510', 1.200, 10.00, 10.00, 20.00, 220.00, NULL, '2026-08-01T11:15:00', '2026-08-01T12:00:00');
SET @o2 = SCOPE_IDENTITY();

INSERT INTO Orders (OrderNumber, CustomerId, OriginBranchId, Status, ServiceType, PickupType, PickupAddress, PickupScheduledAt, RecipientName, RecipientPhone, RecipientAddress, RecipientCity, RecipientState, RecipientZipCode, DeclaredWeightKg, DeclaredWidthCm, DeclaredHeightCm, DeclaredLengthCm, QuotedPrice, Notes, CreatedAt, UpdatedAt)
VALUES ('ORD-SEED0003', @c3, NULL, 'Converted', 'Standard', 'HomePickup', N'Av. Chapultepec 89', '2026-08-02T09:00:00', N'Diego Castro', '+52-33-4000-0003', N'Calle Hidalgo 10', N'Guadalajara', N'Jalisco', '44200', 5.000, 30.00, 25.00, 40.00, 180.00, NULL, '2026-08-01T11:30:00', '2026-08-02T09:30:00');
SET @o3 = SCOPE_IDENTITY();

INSERT INTO Orders (OrderNumber, CustomerId, OriginBranchId, Status, ServiceType, PickupType, PickupAddress, PickupScheduledAt, RecipientName, RecipientPhone, RecipientAddress, RecipientCity, RecipientState, RecipientZipCode, DeclaredWeightKg, DeclaredWidthCm, DeclaredHeightCm, DeclaredLengthCm, QuotedPrice, Notes, CreatedAt, UpdatedAt)
VALUES ('ORD-SEED0004', @c4, NULL, 'Cancelled', 'Economy', 'HomePickup', N'Av. Industria 300', '2026-08-03T14:00:00', N'Comercializadora Cliente Final', '+52-55-4000-0004', N'Calle Morelos 5', N'Toluca', N'Estado de Mexico', '50000', 15.000, 50.00, 40.00, 60.00, 300.00, N'Cliente canceló por cambio de fecha.', '2026-08-01T11:45:00', '2026-08-03T15:00:00');
SET @o4 = SCOPE_IDENTITY();

INSERT INTO Orders (OrderNumber, CustomerId, OriginBranchId, Status, ServiceType, PickupType, PickupAddress, PickupScheduledAt, RecipientName, RecipientPhone, RecipientAddress, RecipientCity, RecipientState, RecipientZipCode, DeclaredWeightKg, DeclaredWidthCm, DeclaredHeightCm, DeclaredLengthCm, QuotedPrice, Notes, CreatedAt, UpdatedAt)
VALUES ('ORD-SEED0005', @c5, @b2, 'Pending', 'Standard', 'DropOff', NULL, NULL, N'Empresa Receptora SA', '+52-81-4000-0005', N'Parque Industrial 200', N'Monterrey', N'Nuevo Leon', '64200', 25.000, 60.00, 50.00, 80.00, 450.00, NULL, '2026-08-01T12:00:00', NULL);
SET @o5 = SCOPE_IDENTITY();

DECLARE @s1 INT, @s2 INT, @s3 INT, @s4 INT, @s5 INT;

-- ===== Shipments (5) =====
INSERT INTO Shipments (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt, OrderId)
VALUES ('TRK-SEED0001', N'Diego Castro', 'Collected', '2026-08-02T09:30:00', NULL, @o3);
SET @s1 = SCOPE_IDENTITY();

INSERT INTO Shipments (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt, OrderId)
VALUES ('TRK-SEED0002', N'Lucia Mendoza', 'InTransit', '2026-08-02T10:00:00', NULL, NULL);
SET @s2 = SCOPE_IDENTITY();

INSERT INTO Shipments (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt, OrderId)
VALUES ('TRK-SEED0003', N'Pedro Ramirez', 'OutForDelivery', '2026-08-02T11:00:00', NULL, NULL);
SET @s3 = SCOPE_IDENTITY();

INSERT INTO Shipments (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt, OrderId)
VALUES ('TRK-SEED0004', N'Empresa Receptora SA', 'Delivered', '2026-07-30T08:00:00', '2026-08-01T16:00:00', NULL);
SET @s4 = SCOPE_IDENTITY();

INSERT INTO Shipments (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt, OrderId)
VALUES ('TRK-SEED0005', N'Cliente Cancelado', 'Cancelled', '2026-08-01T09:00:00', NULL, NULL);
SET @s5 = SCOPE_IDENTITY();

-- ===== ShipmentEvents (5) =====
INSERT INTO ShipmentEvents (ShipmentId, EventType, StatusSnapshot, OccurredAt, EmployeeId, LocationLabel, Notes, CreatedAt)
VALUES (@s1, 'OrderConverted', 'Collected', '2026-08-02T09:30:00', NULL, N'Sucursal Sur', NULL, '2026-08-02T09:30:00');

INSERT INTO ShipmentEvents (ShipmentId, EventType, StatusSnapshot, OccurredAt, EmployeeId, LocationLabel, Notes, CreatedAt)
VALUES (@s2, 'InTransit', 'InTransit', '2026-08-02T12:00:00', @e4, N'Hub Norte', N'Salió en ruta hacia CDMX.', '2026-08-02T12:00:00');

INSERT INTO ShipmentEvents (ShipmentId, EventType, StatusSnapshot, OccurredAt, EmployeeId, LocationLabel, Notes, CreatedAt)
VALUES (@s3, 'OutForDelivery', 'OutForDelivery', '2026-08-02T13:00:00', @e3, N'Sucursal Norte', NULL, '2026-08-02T13:00:00');

INSERT INTO ShipmentEvents (ShipmentId, EventType, StatusSnapshot, OccurredAt, EmployeeId, LocationLabel, Notes, CreatedAt)
VALUES (@s4, 'ReceivedAtBranch', 'Delivered', '2026-07-30T08:15:00', @e4, N'Sucursal Norte', N'Recibido en almacén antes de entrega final.', '2026-07-30T08:15:00');

INSERT INTO ShipmentEvents (ShipmentId, EventType, StatusSnapshot, OccurredAt, EmployeeId, LocationLabel, Notes, CreatedAt)
VALUES (@s5, 'DepartedFromBranch', 'Cancelled', '2026-08-01T09:30:00', @e2, N'Sucursal Centro', N'Envío cancelado tras salida.', '2026-08-01T09:30:00');
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte en orden inverso de dependencia, todo identificado por sus claves de negocio
            // (números de guía/orden, plates, emails, etc.) — nunca por Id, que no es determinístico.
            migrationBuilder.Sql(
@"
DELETE FROM ShipmentEvents WHERE ShipmentId IN (
    SELECT Id FROM Shipments WHERE TrackingNumber IN ('TRK-SEED0001','TRK-SEED0002','TRK-SEED0003','TRK-SEED0004','TRK-SEED0005')
);

DELETE FROM Shipments WHERE TrackingNumber IN ('TRK-SEED0001','TRK-SEED0002','TRK-SEED0003','TRK-SEED0004','TRK-SEED0005');

DELETE FROM Orders WHERE OrderNumber IN ('ORD-SEED0001','ORD-SEED0002','ORD-SEED0003','ORD-SEED0004','ORD-SEED0005');

DELETE FROM IndividualCustomers WHERE GovernmentId IN ('SEEDGOVID0001','SEEDGOVID0002','SEEDGOVID0003');
DELETE FROM BusinessCustomers WHERE TaxId IN ('CXY990101AB1','DAB980202CD2');
DELETE FROM Customers WHERE Email IN (
    'laura.hernandez.seed@example.com','miguel.sanchez.seed@example.com','patricia.flores.seed@example.com',
    'contacto.comercializadoraxyz.seed@example.com','contacto.distribuidoraabc.seed@example.com'
);

DELETE FROM Vehicles WHERE Plate IN ('SEED-001','SEED-002','SEED-003','SEED-004','SEED-005');

DELETE FROM Employees WHERE EmployeeNumber IN ('SEED-E0001','SEED-E0002','SEED-E0003','SEED-E0004','SEED-E0005');

DELETE FROM Branches WHERE Name IN ('Sucursal Centro','Sucursal Norte','Sucursal Sur','Punto de Recoleccion Poniente','Oficina Corporativa')
    AND Address IN ('Av. Reforma 100','Av. Constitucion 500','Av. Vallarta 200','Calle Poniente 45','Paseo de la Reforma 350');
"
            );
        }
    }
}
