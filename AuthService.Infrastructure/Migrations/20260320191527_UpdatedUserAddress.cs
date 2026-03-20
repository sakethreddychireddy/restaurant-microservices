using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "UsersAddresses",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "UsersAddresses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UsersAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "UsersAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "UsersAddresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "UsersAddresses");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UsersAddresses");

            migrationBuilder.DropColumn(
                name: "State",
                table: "UsersAddresses");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "UsersAddresses");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "UsersAddresses",
                newName: "FullAddress");
        }
    }
}
