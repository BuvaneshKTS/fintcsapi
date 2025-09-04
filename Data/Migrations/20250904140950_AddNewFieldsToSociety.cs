using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintcsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToSociety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "chBounceCharge",
                table: "Societies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dropdownArray",
                table: "Societies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "targetDropdown",
                table: "Societies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chBounceCharge",
                table: "Societies");

            migrationBuilder.DropColumn(
                name: "dropdownArray",
                table: "Societies");

            migrationBuilder.DropColumn(
                name: "targetDropdown",
                table: "Societies");
        }
    }
}
