using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PayrollService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDbConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "TaxBrackets",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "AgeBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    EmployeeRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EmployerRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeBrackets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlagEmoji = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocialContributionLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocialContributionEmployeeRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SocialContributionEmployerRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HasProgressiveTax = table.Column<bool>(type: "bit", nullable: false),
                    HasAgeBased = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryPolicies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AgeBrackets",
                columns: new[] { "Id", "CountryCode", "Description", "EmployeeRate", "EmployerRate", "MaxAge", "MinAge" },
                values: new object[,]
                {
                    { 1, "SG", "Age 55 and below", 20m, 17m, 55, 0 },
                    { 2, "SG", "Age 56 to 60", 15m, 13m, 60, 56 },
                    { 3, "SG", "Age 61 to 65", 9.5m, 9m, 65, 61 },
                    { 4, "SG", "Age above 65", 7m, 7.5m, 999, 66 }
                });

            migrationBuilder.InsertData(
                table: "CountryPolicies",
                columns: new[] { "Id", "CountryCode", "CountryName", "CreatedAt", "Currency", "FlagEmoji", "HasAgeBased", "HasProgressiveTax", "IsActive", "SocialContributionEmployeeRate", "SocialContributionEmployerRate", "SocialContributionLabel" },
                values: new object[,]
                {
                    { 1, "MM", "Myanmar", new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1419), "MMK", "🇲🇲", false, true, true, 2m, 3m, "SSB" },
                    { 2, "SG", "Singapore", new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1431), "SGD", "🇸🇬", true, false, true, 0m, 0m, "CPF" },
                    { 3, "TH", "Thailand", new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1433), "THB", "🇹🇭", false, true, true, 5m, 5m, "SSF" },
                    { 4, "MY", "Malaysia", new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1434), "MYR", "🇲🇾", false, true, true, 11m, 13m, "EPF" },
                    { 5, "PH", "Philippines", new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1436), "PHP", "🇵🇭", false, true, true, 4.5m, 9.5m, "SSS" }
                });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Country",
                value: "MM");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Country",
                value: "MM");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 3,
                column: "Country",
                value: "MM");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 4,
                column: "Country",
                value: "MM");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 5,
                column: "Country",
                value: "MM");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Country", "Description", "MaxIncome", "TaxRate" },
                values: new object[] { "TH", "0% - Up to 150,000 THB", 150000m, 0m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "5% - 150K to 300K THB", 300000m, 150001m, 5m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "10% - 300K to 500K THB", 500000m, 300001m, 10m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "15% - 500K to 750K THB", 750000m, 500001m, 15m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "20% - 750K to 1M THB", 1000000m, 750001m, 20m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "25% - 1M to 2M THB", 2000000m, 1000001m, 25m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Country", "Description", "MinIncome", "TaxRate" },
                values: new object[] { "TH", "35% - Above 2M THB", 2000001m, 35m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Country", "Description", "MaxIncome", "TaxRate" },
                values: new object[] { "MY", "0% - Up to 5,000 MYR", 5000m, 0m });

            migrationBuilder.InsertData(
                table: "TaxBrackets",
                columns: new[] { "Id", "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[,]
                {
                    { 14, "MY", "1% - 5K to 20K MYR", 20000m, 5001m, 1m },
                    { 15, "MY", "3% - 20K to 35K MYR", 35000m, 20001m, 3m },
                    { 16, "MY", "8% - 35K to 50K MYR", 50000m, 35001m, 8m },
                    { 17, "MY", "13% - Above 50K MYR", 999999999m, 50001m, 13m },
                    { 18, "PH", "0% - Up to 250,000 PHP", 250000m, 0m, 0m },
                    { 19, "PH", "20% - 250K to 400K PHP", 400000m, 250001m, 20m },
                    { 20, "PH", "25% - 400K to 800K PHP", 800000m, 400001m, 25m },
                    { 21, "PH", "30% - 800K to 2M PHP", 2000000m, 800001m, 30m },
                    { 22, "PH", "32% - 2M to 8M PHP", 8000000m, 2000001m, 32m },
                    { 23, "PH", "35% - Above 8M PHP", 999999999m, 8000001m, 35m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeBrackets");

            migrationBuilder.DropTable(
                name: "CountryPolicies");

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "TaxBrackets",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Country",
                value: "Myanmar");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Country",
                value: "Myanmar");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 3,
                column: "Country",
                value: "Myanmar");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 4,
                column: "Country",
                value: "Myanmar");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 5,
                column: "Country",
                value: "Myanmar");

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Country", "Description", "MaxIncome", "TaxRate" },
                values: new object[] { "Singapore_Employee_55below", "CPF Employee 20% (age 55 and below)", 999999999m, 20m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employer_55below", "CPF Employer 17% (age 55 and below)", 999999999m, 0m, 17m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employee_55to60", "CPF Employee 15% (age 55-60)", 999999999m, 0m, 15m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employer_55to60", "CPF Employer 13% (age 55-60)", 999999999m, 0m, 13m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employee_60to65", "CPF Employee 9.5% (age 60-65)", 999999999m, 0m, 9.5m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employer_60to65", "CPF Employer 9% (age 60-65)", 999999999m, 0m, 9m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Country", "Description", "MinIncome", "TaxRate" },
                values: new object[] { "Singapore_Employee_65above", "CPF Employee 7% (age above 65)", 0m, 7m });

            migrationBuilder.UpdateData(
                table: "TaxBrackets",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Country", "Description", "MaxIncome", "TaxRate" },
                values: new object[] { "Singapore_Employer_65above", "CPF Employer 7.5% (age above 65)", 999999999m, 7.5m });
        }
    }
}
