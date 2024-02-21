using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class idcolmsAdded1156 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_V_VoteSummarys",
                table: "V_VoteSummarys");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "V_VoteSummarys",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_V_VoteSummarys",
                table: "V_VoteSummarys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_V_VoteSummarys_ConstituencyId",
                table: "V_VoteSummarys",
                column: "ConstituencyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_V_VoteSummarys",
                table: "V_VoteSummarys");

            migrationBuilder.DropIndex(
                name: "IX_V_VoteSummarys_ConstituencyId",
                table: "V_VoteSummarys");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "V_VoteSummarys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_V_VoteSummarys",
                table: "V_VoteSummarys",
                column: "ConstituencyId");
        }
    }
}
