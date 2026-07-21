using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KVS_API.Migrations
{
    /// <inheritdoc />
    public partial class AddKontobewegungTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kontobewegungen",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kontonummer = table.Column<string>(type: "text", nullable: false),
                    typ = table.Column<string>(type: "text", nullable: false),
                    betrag = table.Column<decimal>(type: "numeric", nullable: false),
                    saldonachher = table.Column<decimal>(type: "numeric", nullable: false),
                    gegenkonto = table.Column<string>(type: "text", nullable: true),
                    ausgefuehrtvon = table.Column<Guid>(type: "uuid", nullable: false),
                    zeitpunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    beschreibung = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kontobewegungen", x => x.id);
                    table.ForeignKey(
                        name: "FK_kontobewegungen_konten_kontonummer",
                        column: x => x.kontonummer,
                        principalTable: "konten",
                        principalColumn: "kontonummer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kontobewegungen_kontonummer_zeitpunkt",
                table: "kontobewegungen",
                columns: new[] { "kontonummer", "zeitpunkt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kontobewegungen");
        }
    }
}
