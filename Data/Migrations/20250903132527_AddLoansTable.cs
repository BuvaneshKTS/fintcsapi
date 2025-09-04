using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FintcsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SocietyApprovals_SocietyId",
                table: "SocietyApprovals");

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoanNo = table.Column<string>(type: "text", nullable: false),
                    LoanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoanType = table.Column<string>(type: "text", nullable: false),
                    CustomType = table.Column<string>(type: "text", nullable: true),
                    MemberNo = table.Column<string>(type: "text", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PreviousLoan = table.Column<decimal>(type: "numeric", nullable: false),
                    Installments = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    AuthorizedBy = table.Column<string>(type: "text", nullable: false),
                    PaymentMode = table.Column<string>(type: "text", nullable: false),
                    Bank = table.Column<string>(type: "text", nullable: true),
                    ChequeNo = table.Column<string>(type: "text", nullable: true),
                    ChequeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NetLoan = table.Column<decimal>(type: "numeric", nullable: false),
                    InstallmentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NewLoanShare = table.Column<decimal>(type: "numeric", nullable: false),
                    PayAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocietyApprovals_SocietyId_UserId",
                table: "SocietyApprovals",
                columns: new[] { "SocietyId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_SocietyApprovals_SocietyId_UserId",
                table: "SocietyApprovals");

            migrationBuilder.CreateIndex(
                name: "IX_SocietyApprovals_SocietyId",
                table: "SocietyApprovals",
                column: "SocietyId");
        }
    }
}
