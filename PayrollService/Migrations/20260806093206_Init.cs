using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PayrollService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeLoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLoans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Allowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimePay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearEndBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThirteenthMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SocialSecurity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cpfemployee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cpfemployer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBrackets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TaxBrackets",
                columns: new[] { "Id", "Country", "Description", "MaxIncome", "MinIncome", "TaxRate" },
                values: new object[,]
                {
                    { 1, "Myanmar", "0% - Up to 4.8M MMK", 4800000m, 0m, 0m },
                    { 2, "Myanmar", "5% - 4.8M to 10M MMK", 10000000m, 4800001m, 5m },
                    { 3, "Myanmar", "10% - 10M to 20M MMK", 20000000m, 10000001m, 10m },
                    { 4, "Myanmar", "15% - 20M to 30M MMK", 30000000m, 20000001m, 15m },
                    { 5, "Myanmar", "20% - Above 30M MMK", 999999999m, 30000001m, 20m },
                    { 6, "Singapore_Employee_55below", "CPF Employee 20% (age 55 and below)", 999999999m, 0m, 20m },
                    { 7, "Singapore_Employer_55below", "CPF Employer 17% (age 55 and below)", 999999999m, 0m, 17m },
                    { 8, "Singapore_Employee_55to60", "CPF Employee 15% (age 55-60)", 999999999m, 0m, 15m },
                    { 9, "Singapore_Employer_55to60", "CPF Employer 13% (age 55-60)", 999999999m, 0m, 13m },
                    { 10, "Singapore_Employee_60to65", "CPF Employee 9.5% (age 60-65)", 999999999m, 0m, 9.5m },
                    { 11, "Singapore_Employer_60to65", "CPF Employer 9% (age 60-65)", 999999999m, 0m, 9m },
                    { 12, "Singapore_Employee_65above", "CPF Employee 7% (age above 65)", 999999999m, 0m, 7m },
                    { 13, "Singapore_Employer_65above", "CPF Employer 7.5% (age above 65)", 999999999m, 0m, 7.5m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeLoans");

            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "TaxBrackets");
        }
    }
}
