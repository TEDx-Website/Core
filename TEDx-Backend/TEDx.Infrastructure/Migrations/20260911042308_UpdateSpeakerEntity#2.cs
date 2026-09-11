using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEDx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpeakerEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTopSpeaker",
                table: "Speakers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTopSpeaker",
                table: "Speakers");
        }
    }
}
