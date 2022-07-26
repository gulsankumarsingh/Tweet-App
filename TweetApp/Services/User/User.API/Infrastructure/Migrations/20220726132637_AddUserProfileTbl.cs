using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace User.API.Infrastructure.Migrations
{
    public partial class AddUserProfileTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    LoginId = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    ContactNumber = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LogoutAt = table.Column<DateTime>(nullable: false),
                    ProfileImg = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.LoginId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
