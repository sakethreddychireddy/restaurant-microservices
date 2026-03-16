using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedEmojiWithImageUrlInTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                table: "MenuItems");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "MenuItems",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "MenuItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                table: "MenuItems",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
