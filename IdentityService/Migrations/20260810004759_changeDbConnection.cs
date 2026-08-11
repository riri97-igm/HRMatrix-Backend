using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class changeDbConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 10, 0, 47, 59, 448, DateTimeKind.Utc).AddTicks(8597), "$2a$11$J2KZqNz5pIlc4cTrMzSuaexb7mf2PyQMMzQNto75wTAPZ8tIu35s6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 6, 9, 20, 37, 904, DateTimeKind.Utc).AddTicks(4311), "$2a$11$sZU9sYz7blRiTb7ce9iri.l80DzV.eluIP9hh1cYRwBYXBjOQo1Cu" });
        }
    }
}
