using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class readcountwritecountDeltaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VotesJsonAsStringDelta",
                table: "V_Votes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadCount",
                table: "V_Constituencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WriteCount",
                table: "V_Constituencies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VotesJsonAsStringDelta",
                table: "V_Votes");

            migrationBuilder.DropColumn(
                name: "ReadCount",
                table: "V_Constituencies");

            migrationBuilder.DropColumn(
                name: "WriteCount",
                table: "V_Constituencies");
        }
    }
}
