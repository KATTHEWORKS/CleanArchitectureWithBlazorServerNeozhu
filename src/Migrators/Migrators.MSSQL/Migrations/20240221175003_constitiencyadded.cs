using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class constitiencyadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConstituencyId1",
                table: "V_VoteSummarys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Constituencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExistingMpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateMpNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    WriteCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constituencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_V_VoteSummarys_ConstituencyId1",
                table: "V_VoteSummarys",
                column: "ConstituencyId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_V_VoteSummarys_Constituencies_ConstituencyId1",
                table: "V_VoteSummarys",
                column: "ConstituencyId1",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V_VoteSummarys_Constituencies_ConstituencyId1",
                table: "V_VoteSummarys");

            migrationBuilder.DropTable(
                name: "Constituencies");

            migrationBuilder.DropIndex(
                name: "IX_V_VoteSummarys_ConstituencyId1",
                table: "V_VoteSummarys");

            migrationBuilder.DropColumn(
                name: "ConstituencyId1",
                table: "V_VoteSummarys");
        }
    }
}
