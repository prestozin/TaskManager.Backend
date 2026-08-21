using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InsertData_TaskPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskPriority",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Baixa" },
                    { 2, "Média" },
                    { 3, "Alta" }
                });

            migrationBuilder.Sql("""
                UPDATE Tasks
                SET PriorityId = 1
                WHERE PriorityId IS NULL
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "PriorityId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "TaskPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "PriorityId",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskPriority_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "TaskPriority",
                principalColumn: "Id");

            migrationBuilder.DeleteData(
                table: "TaskPriority",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    1,
                    2,
                    3
                });
        }
    }
}
