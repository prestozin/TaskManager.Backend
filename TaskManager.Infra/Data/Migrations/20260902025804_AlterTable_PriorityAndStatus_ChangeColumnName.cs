using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AlterTable_PriorityAndStatus_ChangeColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TaskStatus",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TaskPriority",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TaskStatus",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TaskPriority",
                newName: "Description");
        }
    }
}
