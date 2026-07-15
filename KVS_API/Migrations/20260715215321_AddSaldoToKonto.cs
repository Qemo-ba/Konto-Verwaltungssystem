using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KVS_API.Migrations
{
    /// <inheritdoc />
    public partial class AddSaldoToKonto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "saldo",
                table: "konten",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "saldo",
                table: "konten");
        }
    }
}
