using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendShipmentEventsAndAddDeliveryAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShipmentEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "ShipmentEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationLabel",
                table: "ShipmentEvents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ShipmentEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentEventId = table.Column<int>(type: "int", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAttempts_ShipmentEvents_ShipmentEventId",
                        column: x => x.ShipmentEventId,
                        principalTable: "ShipmentEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentEvents_EmployeeId",
                table: "ShipmentEvents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAttempts_ShipmentEventId",
                table: "DeliveryAttempts",
                column: "ShipmentEventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentEvents_Employees_EmployeeId",
                table: "ShipmentEvents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentEvents_Employees_EmployeeId",
                table: "ShipmentEvents");

            migrationBuilder.DropTable(
                name: "DeliveryAttempts");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentEvents_EmployeeId",
                table: "ShipmentEvents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShipmentEvents");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "ShipmentEvents");

            migrationBuilder.DropColumn(
                name: "LocationLabel",
                table: "ShipmentEvents");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ShipmentEvents");
        }
    }
}
