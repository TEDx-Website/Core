using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEDx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryFromSpeakers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Speakers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Speakers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
