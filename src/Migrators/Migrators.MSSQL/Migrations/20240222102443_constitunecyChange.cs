using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class constitunecyChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StateName",
                table: "Constituencies",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "ExistingMpName",
                table: "Constituencies",
                newName: "MpNamesEarlierOthers");

            migrationBuilder.RenameColumn(
                name: "AlternateMpNames",
                table: "Constituencies",
                newName: "MpNameExisting");

            migrationBuilder.AddColumn<string>(
                name: "ExistingMpParty",
                table: "Constituencies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExistingMpTerms",
                table: "Constituencies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExistingMpParty",
                table: "Constituencies");

            migrationBuilder.DropColumn(
                name: "ExistingMpTerms",
                table: "Constituencies");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Constituencies",
                newName: "StateName");

            migrationBuilder.RenameColumn(
                name: "MpNamesEarlierOthers",
                table: "Constituencies",
                newName: "ExistingMpName");

            migrationBuilder.RenameColumn(
                name: "MpNameExisting",
                table: "Constituencies",
                newName: "AlternateMpNames");
        }
    }
}
