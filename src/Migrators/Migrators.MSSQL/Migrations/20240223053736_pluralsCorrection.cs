using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class pluralsCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VoteCountForExistingMp",
                table: "VoteSummaries",
                newName: "VotesCountForExistingMp");

            migrationBuilder.RenameColumn(
                name: "VoteCountAgainstExistingMp",
                table: "VoteSummaries",
                newName: "VotesCountAgainstExistingMp");

            migrationBuilder.RenameColumn(
                name: "VoteCount",
                table: "VoteSummaries",
                newName: "VotesCount");

            migrationBuilder.RenameColumn(
                name: "ReadCount",
                table: "VoteConstituencies",
                newName: "ReadsCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VotesCountForExistingMp",
                table: "VoteSummaries",
                newName: "VoteCountForExistingMp");

            migrationBuilder.RenameColumn(
                name: "VotesCountAgainstExistingMp",
                table: "VoteSummaries",
                newName: "VoteCountAgainstExistingMp");

            migrationBuilder.RenameColumn(
                name: "VotesCount",
                table: "VoteSummaries",
                newName: "VoteCount");

            migrationBuilder.RenameColumn(
                name: "ReadsCount",
                table: "VoteConstituencies",
                newName: "ReadCount");
        }
    }
}
