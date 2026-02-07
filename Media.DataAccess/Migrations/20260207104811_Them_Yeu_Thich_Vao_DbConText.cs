using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Them_Yeu_Thich_Vao_DbConText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YeuThich_KhachHangs_MaKhachHang",
                table: "YeuThich");

            migrationBuilder.DropForeignKey(
                name: "FK_YeuThich_Saches_MaSach",
                table: "YeuThich");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YeuThich",
                table: "YeuThich");

            migrationBuilder.RenameTable(
                name: "YeuThich",
                newName: "YeuThichs");

            migrationBuilder.RenameIndex(
                name: "IX_YeuThich_MaSach",
                table: "YeuThichs",
                newName: "IX_YeuThichs_MaSach");

            migrationBuilder.RenameIndex(
                name: "IX_YeuThich_MaKhachHang",
                table: "YeuThichs",
                newName: "IX_YeuThichs_MaKhachHang");

            migrationBuilder.AddPrimaryKey(
                name: "PK_YeuThichs",
                table: "YeuThichs",
                column: "MaYeuThich");

            migrationBuilder.AddForeignKey(
                name: "FK_YeuThichs_KhachHangs_MaKhachHang",
                table: "YeuThichs",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YeuThichs_Saches_MaSach",
                table: "YeuThichs",
                column: "MaSach",
                principalTable: "Saches",
                principalColumn: "MaSach",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YeuThichs_KhachHangs_MaKhachHang",
                table: "YeuThichs");

            migrationBuilder.DropForeignKey(
                name: "FK_YeuThichs_Saches_MaSach",
                table: "YeuThichs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YeuThichs",
                table: "YeuThichs");

            migrationBuilder.RenameTable(
                name: "YeuThichs",
                newName: "YeuThich");

            migrationBuilder.RenameIndex(
                name: "IX_YeuThichs_MaSach",
                table: "YeuThich",
                newName: "IX_YeuThich_MaSach");

            migrationBuilder.RenameIndex(
                name: "IX_YeuThichs_MaKhachHang",
                table: "YeuThich",
                newName: "IX_YeuThich_MaKhachHang");

            migrationBuilder.AddPrimaryKey(
                name: "PK_YeuThich",
                table: "YeuThich",
                column: "MaYeuThich");

            migrationBuilder.AddForeignKey(
                name: "FK_YeuThich_KhachHangs_MaKhachHang",
                table: "YeuThich",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YeuThich_Saches_MaSach",
                table: "YeuThich",
                column: "MaSach",
                principalTable: "Saches",
                principalColumn: "MaSach",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
