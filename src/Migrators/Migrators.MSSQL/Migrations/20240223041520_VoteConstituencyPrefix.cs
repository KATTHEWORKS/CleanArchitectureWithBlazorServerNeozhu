using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class VoteConstituencyPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Constituencies_ConstituencyId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteSummaries_Constituencies_ConstituencyId",
                table: "VoteSummaries");

            migrationBuilder.DropTable(
                name: "Constituencies");

            migrationBuilder.CreateTable(
                name: "VoteConstituencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MpNameExisting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingMpParty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingMpTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MpNamesEarlierOthers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteConstituencies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_VoteConstituencies_ConstituencyId",
                table: "Votes",
                column: "ConstituencyId",
                principalTable: "VoteConstituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSummaries_VoteConstituencies_ConstituencyId",
                table: "VoteSummaries",
                column: "ConstituencyId",
                principalTable: "VoteConstituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VoteConstituencies_ConstituencyId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteSummaries_VoteConstituencies_ConstituencyId",
                table: "VoteSummaries");

            migrationBuilder.DropTable(
                name: "VoteConstituencies");

            migrationBuilder.CreateTable(
                name: "Constituencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingMpParty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingMpTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MpNameExisting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MpNamesEarlierOthers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constituencies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Constituencies_ConstituencyId",
                table: "Votes",
                column: "ConstituencyId",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSummaries_Constituencies_ConstituencyId",
                table: "VoteSummaries",
                column: "ConstituencyId",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
