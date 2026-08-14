using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveService.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerIdToLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "LeaveRequests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "LeaveRequests");
        }
    }
}
