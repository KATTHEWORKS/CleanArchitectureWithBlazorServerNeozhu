using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class votingchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "V_Votes");

            migrationBuilder.DropTable(
                name: "V_VoteSummarys");

            migrationBuilder.DropTable(
                name: "V_Constituencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "V_Constituencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlternateMpNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExistingMpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadCount = table.Column<int>(type: "int", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WriteCount = table.Column<int>(type: "int", nullable: false)
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
                    ConstituencyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConstituencyIdDelta = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    VoteKPIComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoteKPIRatingComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoteKPIRatingCommentsDelta = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_V_Votes_V_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
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
                    ConstituencyId = table.Column<int>(type: "int", nullable: false),
                    CommentsCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    KPIVotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V_VoteSummarys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V_VoteSummarys_V_Constituencies_ConstituencyId",
                        column: x => x.ConstituencyId,
                        principalTable: "V_Constituencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_V_Votes_ConstituencyId",
                table: "V_Votes",
                column: "ConstituencyId");

            migrationBuilder.CreateIndex(
                name: "IX_V_Votes_UserId",
                table: "V_Votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_V_VoteSummarys_ConstituencyId",
                table: "V_VoteSummarys",
                column: "ConstituencyId",
                unique: true);
        }
    }
}
