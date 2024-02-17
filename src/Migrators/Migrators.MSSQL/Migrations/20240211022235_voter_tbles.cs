using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class voter_tbles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "V_Constituencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Constituency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExistingMpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateMpNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V_Constituencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "V_Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MPId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V_Votes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_V_Votes_V_Constituencies_MPId",
                        column: x => x.MPId,
                        principalTable: "V_Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "V_VoteSummarys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MPId = table.Column<int>(type: "int", nullable: false),
                    CommentCountForMpId = table.Column<int>(type: "int", nullable: false),
                    TotalVoteCountForMpId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V_VoteSummarys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V_VoteSummarys_V_Constituencies_MPId",
                        column: x => x.MPId,
                        principalTable: "V_Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_V_Votes_MPId",
                table: "V_Votes",
                column: "MPId");

            migrationBuilder.CreateIndex(
                name: "IX_V_Votes_UserId",
                table: "V_Votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_V_VoteSummarys_MPId",
                table: "V_VoteSummarys",
                column: "MPId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "V_Votes");

            migrationBuilder.DropTable(
                name: "V_VoteSummarys");

            migrationBuilder.DropTable(
                name: "V_Constituencies");
        }
    }
}
