using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Envio recolectado
            migrationBuilder.Sql(
                "INSERT INTO Shipments(TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt) " +
                "VALUES('TRK-90001', 'Angel', 'Collected', '2026-07-28T08:30:00', NULL);"
            );

            // Envío en transito
            migrationBuilder.Sql(
                "INSERT INTO Shipments(TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt) " +
                "VALUES('TRK-90002', 'Maria Garcia', 'InTransit', '2026-07-27T14:15:00', NULL);"
            );

            // Envio entregado (contiene fecha de entrega)
            migrationBuilder.Sql(
                "INSERT INTO Shipments(TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt) " +
                "VALUES('TRK-90003', 'Carlos Lopez', 'Delivered', '2026-07-20T09:00:00', '2026-07-22T16:45:00');"
            );

            // Envio cancelado
            migrationBuilder.Sql(
                "INSERT INTO Shipments(TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt) " +
                "VALUES('TRK-90004', 'Ana Martinez', 'Cancelled', '2026-07-28T10:20:00', NULL);"
            );

            // Envio en transito
            migrationBuilder.Sql(
                "INSERT INTO Shipments(TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt) " +
                "VALUES('TRK-90005', 'Roberto Gomez', 'InTransit', '2026-07-26T09:10:00', NULL);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte la inserción eliminando los registros por su número de guía
            migrationBuilder.Sql(
                "DELETE FROM Shipments WHERE TrackingNumber IN " +
                "('TRK-90001', 'TRK-90002', 'TRK-90003', 'TRK-90004', 'TRK-90005');"
            );
        }
    }
}
