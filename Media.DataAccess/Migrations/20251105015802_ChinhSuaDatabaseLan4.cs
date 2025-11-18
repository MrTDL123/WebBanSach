using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaDatabaseLan4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Saches",
                newName: "AnhBiaPhu4");

            migrationBuilder.RenameColumn(
                name: "MaNhanXuatBan",
                table: "NhaXuatBans",
                newName: "MaNhaXuatBan");

            migrationBuilder.AlterColumn<string>(
                name: "TenTG",
                table: "TacGias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AnhBiaChinh",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnhBiaPhu1",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnhBiaPhu2",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnhBiaPhu3",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnhBiaChinh",
                table: "Saches");

            migrationBuilder.DropColumn(
                name: "AnhBiaPhu1",
                table: "Saches");

            migrationBuilder.DropColumn(
                name: "AnhBiaPhu2",
                table: "Saches");

            migrationBuilder.DropColumn(
                name: "AnhBiaPhu3",
                table: "Saches");

            migrationBuilder.RenameColumn(
                name: "AnhBiaPhu4",
                table: "Saches",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "MaNhaXuatBan",
                table: "NhaXuatBans",
                newName: "MaNhanXuatBan");

            migrationBuilder.AlterColumn<string>(
                name: "TenTG",
                table: "TacGias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
