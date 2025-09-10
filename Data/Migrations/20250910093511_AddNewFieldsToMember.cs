using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintcsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email2",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile2",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pincode",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Mobile2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Pincode",
                table: "Members");
        }
    }
}
