using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InsertData_Priority_And_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pendente" },
                    { 2, "Em Progresso" },
                    { 3, "Concluída" },
                    { 4, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "TaskPriority",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Baixa" },
                    { 2, "Média" },
                    { 3, "Alta" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    1, 2, 3, 4
                });

            migrationBuilder.DeleteData(
                table: "TaskPriority",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    1, 2, 3
                });
        }
    }
}