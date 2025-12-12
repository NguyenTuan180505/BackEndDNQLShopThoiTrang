using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThoiTrang.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrls",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "VideoUrls",
                table: "Reviews");
        }
    }
}
