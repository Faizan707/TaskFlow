using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddKanbanStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KanbanStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanStages_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KanbanStages_ProjectId",
                table: "KanbanStages",
                column: "ProjectId");

            // Default stages for every existing project
            migrationBuilder.Sql(@"
                INSERT INTO ""KanbanStages"" (""Name"", ""Order"", ""IsDefault"", ""ProjectId"")
                SELECT 'Todo', 1, TRUE, ""Id"" FROM ""Project"";

                INSERT INTO ""KanbanStages"" (""Name"", ""Order"", ""IsDefault"", ""ProjectId"")
                SELECT 'In Progress', 2, TRUE, ""Id"" FROM ""Project"";

                INSERT INTO ""KanbanStages"" (""Name"", ""Order"", ""IsDefault"", ""ProjectId"")
                SELECT 'Done', 3, TRUE, ""Id"" FROM ""Project"";
            ");

            migrationBuilder.AddColumn<int>(
                name: "StageId",
                table: "Tasks",
                type: "integer",
                nullable: true);

            // Point existing tasks to that project's Todo stage
            migrationBuilder.Sql(@"
                UPDATE ""Tasks"" t
                SET ""StageId"" = s.""Id""
                FROM ""KanbanStages"" s
                WHERE s.""ProjectId"" = t.""ProjectId""
                  AND s.""Name"" = 'Todo'
                  AND s.""IsDefault"" = TRUE;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "StageId",
                table: "Tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StageId",
                table: "Tasks",
                column: "StageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_KanbanStages_StageId",
                table: "Tasks",
                column: "StageId",
                principalTable: "KanbanStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_KanbanStages_StageId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "KanbanStages");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StageId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StageId",
                table: "Tasks");
        }
    }
}
