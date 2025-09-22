using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintcsApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLoanNoFromLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomType",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanNo",
                table: "Loans");

            migrationBuilder.AlterColumn<string>(
                name: "Share",
                table: "Members",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Share",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomType",
                table: "Loans",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanNo",
                table: "Loans",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
