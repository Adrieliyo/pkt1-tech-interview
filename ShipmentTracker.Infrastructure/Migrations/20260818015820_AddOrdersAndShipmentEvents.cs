using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndShipmentEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Shipments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OriginBranchId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PickupScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RecipientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecipientAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RecipientCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecipientState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecipientZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeclaredWeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeclaredWidthCm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeclaredHeightCm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeclaredLengthCm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuotedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Branches_OriginBranchId",
                        column: x => x.OriginBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentEvents_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipments",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OriginBranchId",
                table: "Orders",
                column: "OriginBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentEvents_ShipmentId",
                table: "ShipmentEvents",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Orders_OrderId",
                table: "Shipments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Orders_OrderId",
                table: "Shipments");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ShipmentEvents");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Shipments");
        }
    }
}
