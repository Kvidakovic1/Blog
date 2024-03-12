using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogZavrsni.Data.Migrations
{
    public partial class upload_slika : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Post");
        }
    }
}
