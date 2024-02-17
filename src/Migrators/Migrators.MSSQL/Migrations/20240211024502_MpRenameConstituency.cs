using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class MpRenameConstituency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V_Votes_V_Constituencies_MPId",
                table: "V_Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_V_VoteSummarys_V_Constituencies_MPId",
                table: "V_VoteSummarys");

            migrationBuilder.RenameColumn(
                name: "MPId",
                table: "V_VoteSummarys",
                newName: "ConstituencyId");

            migrationBuilder.RenameIndex(
                name: "IX_V_VoteSummarys_MPId",
                table: "V_VoteSummarys",
                newName: "IX_V_VoteSummarys_ConstituencyId");

            migrationBuilder.RenameColumn(
                name: "MPId",
                table: "V_Votes",
                newName: "ConstituencyId");

            migrationBuilder.RenameIndex(
                name: "IX_V_Votes_MPId",
                table: "V_Votes",
                newName: "IX_V_Votes_ConstituencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_V_Votes_V_Constituencies_ConstituencyId",
                table: "V_Votes",
                column: "ConstituencyId",
                principalTable: "V_Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_V_VoteSummarys_V_Constituencies_ConstituencyId",
                table: "V_VoteSummarys",
                column: "ConstituencyId",
                principalTable: "V_Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V_Votes_V_Constituencies_ConstituencyId",
                table: "V_Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_V_VoteSummarys_V_Constituencies_ConstituencyId",
                table: "V_VoteSummarys");

            migrationBuilder.RenameColumn(
                name: "ConstituencyId",
                table: "V_VoteSummarys",
                newName: "MPId");

            migrationBuilder.RenameIndex(
                name: "IX_V_VoteSummarys_ConstituencyId",
                table: "V_VoteSummarys",
                newName: "IX_V_VoteSummarys_MPId");

            migrationBuilder.RenameColumn(
                name: "ConstituencyId",
                table: "V_Votes",
                newName: "MPId");

            migrationBuilder.RenameIndex(
                name: "IX_V_Votes_ConstituencyId",
                table: "V_Votes",
                newName: "IX_V_Votes_MPId");

            migrationBuilder.AddForeignKey(
                name: "FK_V_Votes_V_Constituencies_MPId",
                table: "V_Votes",
                column: "MPId",
                principalTable: "V_Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_V_VoteSummarys_V_Constituencies_MPId",
                table: "V_VoteSummarys",
                column: "MPId",
                principalTable: "V_Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
