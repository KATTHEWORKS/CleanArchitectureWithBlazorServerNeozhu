using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class dbJson123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsJsonAsString",
                table: "V_Votes");

            migrationBuilder.DropColumn(
                name: "VotesJsonAsString",
                table: "V_Votes");

            migrationBuilder.DropColumn(
                name: "VotesJsonAsStringDelta",
                table: "V_Votes");

            migrationBuilder.AddColumn<string>(
                name: "VoteKPIComments",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VoteKPIRatingComments",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteKPIComments",
                table: "V_Votes");

            migrationBuilder.DropColumn(
                name: "VoteKPIRatingComments",
                table: "V_Votes");

            migrationBuilder.AddColumn<string>(
                name: "CommentsJsonAsString",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesJsonAsString",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesJsonAsStringDelta",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
