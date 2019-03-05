using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeGenerator.Data.Migrations
{
    public partial class Migration00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CodeGeneratorTemplates");

            migrationBuilder.RenameColumn(
                name: "Context",
                table: "CodeGeneratorTemplates",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CodeGeneratorGenerators",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CodeGeneratorGenerators");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "CodeGeneratorTemplates",
                newName: "Context");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CodeGeneratorTemplates",
                maxLength: 500,
                nullable: true);
        }
    }
}
