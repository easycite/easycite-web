using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyCite.Migrations
{
    public partial class AddReferenceIsPending : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPending",
                table: "ProjectReferences",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPending",
                table: "ProjectReferences");
        }
    }
}
