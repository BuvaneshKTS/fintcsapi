using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintcsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSocietyDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cdAmount",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cdAmount",
                table: "Members");
        }
    }
}
