using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEDx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoverOrderEventStatusForSeatAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_Event_Status",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Event_Status",
                table: "Orders",
                columns: new[] { "EventId", "Status" })
                .Annotation("SqlServer:Include", new[] { "Quantity", "HoldExpiresAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_Event_Status",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Event_Status",
                table: "Orders",
                columns: new[] { "EventId", "Status" });
        }
    }
}
