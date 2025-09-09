using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintcsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanTypesToSociety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoanTypes",
                table: "Societies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanTypes",
                table: "Societies");
        }
    }
}
