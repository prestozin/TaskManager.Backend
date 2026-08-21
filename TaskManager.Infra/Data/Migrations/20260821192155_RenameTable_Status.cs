using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Status_StatusId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Status",
                table: "Status");

            migrationBuilder.RenameTable(
                name: "Status",
                newName: "TaskStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatus_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "TaskStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatus_StatusId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskStatus",
                table: "TaskStatus");

            migrationBuilder.RenameTable(
                name: "TaskStatus",
                newName: "Status");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Status",
                table: "Status",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Status_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
