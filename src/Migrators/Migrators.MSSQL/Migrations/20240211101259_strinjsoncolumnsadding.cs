using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class strinjsoncolumnsadding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KPIVotesAsJsonString",
                table: "V_VoteSummarys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KPIVotesAsJsonString",
                table: "V_VoteSummarys");

            migrationBuilder.DropColumn(
                name: "CommentsJsonAsString",
                table: "V_Votes");

            migrationBuilder.DropColumn(
                name: "VotesJsonAsString",
                table: "V_Votes");
        }
    }
}
