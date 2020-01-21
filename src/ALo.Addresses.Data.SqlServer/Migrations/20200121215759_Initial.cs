using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ALo.Addresses.Data.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    ParentAddressId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 120, nullable: false),
                    TypeShortName = table.Column<string>(maxLength: 10, nullable: false),
                    ActualityStatus = table.Column<byte>(nullable: false),
                    Level = table.Column<byte>(nullable: false),
                    DivisionType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HouseId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    HouseNumber = table.Column<string>(maxLength: 20, nullable: true),
                    BuildNumber = table.Column<string>(maxLength: 10, nullable: true),
                    StructureNumber = table.Column<string>(maxLength: 10, nullable: true),
                    HouseType = table.Column<byte>(nullable: false),
                    HouseState = table.Column<byte>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AddressId",
                schema: "dbo",
                table: "Addresses",
                column: "AddressId")
                .Annotation("SqlServer:Include", new[] { "ActualityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ParentAddressId",
                schema: "dbo",
                table: "Addresses",
                column: "ParentAddressId")
                .Annotation("SqlServer:Include", new[] { "ActualityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Houses_AddressId",
                schema: "dbo",
                table: "Houses",
                column: "AddressId")
                .Annotation("SqlServer:Include", new[] { "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Houses_HouseId",
                schema: "dbo",
                table: "Houses",
                column: "HouseId")
                .Annotation("SqlServer:Include", new[] { "EndDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Houses",
                schema: "dbo");
        }
    }
}
