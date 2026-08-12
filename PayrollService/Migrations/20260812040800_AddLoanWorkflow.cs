using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollService.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedDate",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CFOApprovedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CFOApprovedBy",
                table: "EmployeeLoans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CFOApprovedByName",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CFOComment",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HRApprovedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HRApprovedBy",
                table: "EmployeeLoans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRApprovedByName",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HRComment",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LoanType",
                table: "EmployeeLoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManagerApprovedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerApprovedBy",
                table: "EmployeeLoans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerApprovedByName",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManagerComment",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "EmployeeLoans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByName",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EmployeeLoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RepaymentMonths",
                table: "EmployeeLoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount",
                table: "EmployeeLoans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EmployeeLoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 12, 4, 7, 57, 831, DateTimeKind.Utc).AddTicks(2661));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 12, 4, 7, 57, 831, DateTimeKind.Utc).AddTicks(2694));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 12, 4, 7, 57, 831, DateTimeKind.Utc).AddTicks(2695));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 12, 4, 7, 57, 831, DateTimeKind.Utc).AddTicks(2696));

            migrationBuilder.UpdateData(
                table: "CountryPolicies",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 12, 4, 7, 57, 831, DateTimeKind.Utc).AddTicks(2698));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedDate",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CFOApprovedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CFOApprovedBy",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CFOApprovedByName",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CFOComment",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "HRApprovedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "HRApprovedBy",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "HRApprovedByName",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "HRComment",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "LoanType",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedBy",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedByName",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "ManagerComment",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "RejectedByName",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "RepaymentMonths",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "EmployeeLoans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeLoans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "EmployeeLoans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
    }
}
