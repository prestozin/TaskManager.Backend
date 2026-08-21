using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable_TaskPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskPriority",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriority", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "TaskPriority",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskPriority");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PriorityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Tasks");
        }
    }
}
