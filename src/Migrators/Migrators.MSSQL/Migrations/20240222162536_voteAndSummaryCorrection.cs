using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class voteAndSummaryCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WriteCount",
                table: "Constituencies");

            migrationBuilder.AddColumn<short>(
                name: "Rating",
                table: "VoteSummaries",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "VoteSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoteCountAgainstExistingMp",
                table: "VoteSummaries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteCountForExistingMp",
                table: "VoteSummaries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenIssues",
                table: "Votes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WishToReElectMp",
                table: "Votes",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "VoteSummaries");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "VoteSummaries");

            migrationBuilder.DropColumn(
                name: "VoteCountAgainstExistingMp",
                table: "VoteSummaries");

            migrationBuilder.DropColumn(
                name: "VoteCountForExistingMp",
                table: "VoteSummaries");

            migrationBuilder.DropColumn(
                name: "OpenIssues",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "WishToReElectMp",
                table: "Votes");

            migrationBuilder.AddColumn<int>(
                name: "WriteCount",
                table: "Constituencies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
