using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEDx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpeakerEntity_AddTopSpeakerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Speakers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNextSpeaker",
                table: "Speakers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "Speakers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Speakers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Speakers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TalkTitle",
                table: "Speakers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopSpeakerOrderIndex",
                table: "Speakers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Track",
                table: "Speakers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Speakers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XUrl",
                table: "Speakers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "IsNextSpeaker",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "TalkTitle",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "TopSpeakerOrderIndex",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "Track",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "XUrl",
                table: "Speakers");
        }
    }
}
