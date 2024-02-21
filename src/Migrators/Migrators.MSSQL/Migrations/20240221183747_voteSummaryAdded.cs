using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class voteSummaryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteSummary_Constituencies_ConstituencyId",
                table: "VoteSummary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteSummary",
                table: "VoteSummary");

            migrationBuilder.RenameTable(
                name: "VoteSummary",
                newName: "VoteSummaries");

            migrationBuilder.RenameIndex(
                name: "IX_VoteSummary_ConstituencyId",
                table: "VoteSummaries",
                newName: "IX_VoteSummaries_ConstituencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteSummaries",
                table: "VoteSummaries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSummaries_Constituencies_ConstituencyId",
                table: "VoteSummaries",
                column: "ConstituencyId",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteSummaries_Constituencies_ConstituencyId",
                table: "VoteSummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteSummaries",
                table: "VoteSummaries");

            migrationBuilder.RenameTable(
                name: "VoteSummaries",
                newName: "VoteSummary");

            migrationBuilder.RenameIndex(
                name: "IX_VoteSummaries_ConstituencyId",
                table: "VoteSummary",
                newName: "IX_VoteSummary_ConstituencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteSummary",
                table: "VoteSummary",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSummary_Constituencies_ConstituencyId",
                table: "VoteSummary",
                column: "ConstituencyId",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
