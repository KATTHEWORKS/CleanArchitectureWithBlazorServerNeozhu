using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class summaryJsonModifuation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VotesCount",
                table: "V_VoteSummarys");

            migrationBuilder.RenameColumn(
                name: "KPIVotesAsJsonString",
                table: "V_VoteSummarys",
                newName: "KPIVotes");

            migrationBuilder.AddColumn<int>(
                name: "ConstituencyIdDelta",
                table: "V_Votes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstituencyIdDelta",
                table: "V_Votes");

            migrationBuilder.RenameColumn(
                name: "KPIVotes",
                table: "V_VoteSummarys",
                newName: "KPIVotesAsJsonString");

            migrationBuilder.AddColumn<int>(
                name: "VotesCount",
                table: "V_VoteSummarys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
