using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class voteAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V_VoteSummarys_Constituencies_ConstituencyId1",
                table: "V_VoteSummarys");

            migrationBuilder.DropIndex(
                name: "IX_V_VoteSummarys_ConstituencyId1",
                table: "V_VoteSummarys");

            migrationBuilder.DropColumn(
                name: "ConstituencyId1",
                table: "V_VoteSummarys");

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConstituencyId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    KPIRatingCommentsDelta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConstituencyIdDelta = table.Column<int>(type: "int", nullable: true),
                    KPIComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KPIRatingComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
                        principalTable: "Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteSummary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConstituencyId = table.Column<int>(type: "int", nullable: false),
                    CommentsCount = table.Column<int>(type: "int", nullable: false),
                    KPIVotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteSummary_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
                        principalTable: "Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ConstituencyId",
                table: "Votes",
                column: "ConstituencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteSummary_ConstituencyId",
                table: "VoteSummary",
                column: "ConstituencyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "VoteSummary");

            migrationBuilder.AddColumn<int>(
                name: "ConstituencyId1",
                table: "V_VoteSummarys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_V_VoteSummarys_ConstituencyId1",
                table: "V_VoteSummarys",
                column: "ConstituencyId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_V_VoteSummarys_Constituencies_ConstituencyId1",
                table: "V_VoteSummarys",
                column: "ConstituencyId1",
                principalTable: "Constituencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
