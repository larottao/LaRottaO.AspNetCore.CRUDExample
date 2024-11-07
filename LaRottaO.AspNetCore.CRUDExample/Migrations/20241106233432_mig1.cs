using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaRottaO.AspNetCore.CRUDExample.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "collaborator_table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassportNumber = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntryCreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collaborator_table", x => x.Id);
                    table.UniqueConstraint("AK_collaborator_table_PassportNumber", x => x.PassportNumber);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "collaborator_data_table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassportNumber = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OverTimeStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OverTimeEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActivitySummary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HadBreakfast = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HadLunch = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HadDinner = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EntryCreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collaborator_data_table", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collaborator_data_table_collaborator_table_PassportNumber",
                        column: x => x.PassportNumber,
                        principalTable: "collaborator_table",
                        principalColumn: "PassportNumber",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_collaborator_data_table_PassportNumber",
                table: "collaborator_data_table",
                column: "PassportNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collaborator_data_table");

            migrationBuilder.DropTable(
                name: "collaborator_table");
        }
    }
}
