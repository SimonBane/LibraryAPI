using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Library.Web.Migrations
{
    public partial class ImplementLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomExceptions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CustomInnerMessage = table.Column<string>(nullable: true),
                    CustomInnerStackTrace = table.Column<string>(nullable: true),
                    CustomMessage = table.Column<string>(nullable: true),
                    CustomStackTrace = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    HResult = table.Column<int>(nullable: false),
                    HelpLink = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomExceptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomExceptions");
        }
    }
}
