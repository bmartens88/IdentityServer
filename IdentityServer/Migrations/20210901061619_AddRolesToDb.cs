using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Migrations
{
    public partial class AddRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "37efe8b9-c046-4385-9aef-2cd3ccba8107", "3362da0a-9aef-4396-a7aa-cd1c68ee2c69", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "989a9499-da77-4ec0-8b10-a96ada8ea742", "aafb4355-0c02-4dd8-ad05-4962256f9b6f", "Visitor", "VISITOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37efe8b9-c046-4385-9aef-2cd3ccba8107");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "989a9499-da77-4ec0-8b10-a96ada8ea742");
        }
    }
}
