using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ThemSlugVaFillPathChoChuDe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DuongDanURL",
                table: "ChuDes",
                newName: "Slug");

            migrationBuilder.AddColumn<string>(
                name: "FullPath",
                table: "ChuDes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullPath",
                table: "ChuDes");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "ChuDes",
                newName: "DuongDanURL");
        }
    }
}
