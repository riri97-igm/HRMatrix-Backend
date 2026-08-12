using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollService.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Payslips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UnpaidLeaveDays",
                table: "Payslips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnpaidLeaveDeduction",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 11, 4, 35, 47, 129, DateTimeKind.Utc).AddTicks(2683));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 11, 4, 35, 47, 129, DateTimeKind.Utc).AddTicks(2722));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 11, 4, 35, 47, 129, DateTimeKind.Utc).AddTicks(2723));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 11, 4, 35, 47, 129, DateTimeKind.Utc).AddTicks(2725));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 11, 4, 35, 47, 129, DateTimeKind.Utc).AddTicks(2726));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "UnpaidLeaveDays",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "UnpaidLeaveDeduction",
                table: "Payslips");

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1419));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1431));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1433));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1434));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 10, 0, 57, 5, 978, DateTimeKind.Utc).AddTicks(1436));
        }
    }
}
