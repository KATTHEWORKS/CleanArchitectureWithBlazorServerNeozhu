using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class townEntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Towns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    District = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    State = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UrlName1 = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UrlName2 = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    GoogleMapAddressUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EndDateToShow = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriotiryOrder = table.Column<int>(type: "int", nullable: true),
                    GoogleProfileUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FaceBookUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    OtherReferenceUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfProfileMasterDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfProfileMasterDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TownProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeOfProfileId = table.Column<int>(type: "int", nullable: false),
                    TownId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    GoogleMapAddressUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EndDateToShow = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriotiryOrder = table.Column<int>(type: "int", nullable: true),
                    GoogleProfileUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FaceBookUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    OtherReferenceUrl = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedCount = table.Column<int>(type: "int", nullable: false),
                    RejectedCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    DisLikeCount = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TownProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TownProfiles_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "Towns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TownProfiles_TypeOfProfileMasterDatas_TypeOfProfileId",
                        column: x => x.TypeOfProfileId,
                        principalTable: "TypeOfProfileMasterDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TownProfiles_TownId",
                table: "TownProfiles",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_TownProfiles_TypeOfProfileId",
                table: "TownProfiles",
                column: "TypeOfProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TownProfiles");

            migrationBuilder.DropTable(
                name: "Towns");

            migrationBuilder.DropTable(
                name: "TypeOfProfileMasterDatas");
        }
    }
}
