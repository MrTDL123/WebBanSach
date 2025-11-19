using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class themdiachinhanhangvaodbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaChiNhanHang_KhachHangs_MaKhachHang",
                table: "DiaChiNhanHang");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_DiaChiNhanHang_MaDiaChi",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiKhachHang_KhachHangs_MaKhachHang",
                table: "PhanHoiKhachHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanHoiKhachHang",
                table: "PhanHoiKhachHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiaChiNhanHang",
                table: "DiaChiNhanHang");

            migrationBuilder.RenameTable(
                name: "PhanHoiKhachHang",
                newName: "PhanHoiKhachHangs");

            migrationBuilder.RenameTable(
                name: "DiaChiNhanHang",
                newName: "DiaChiNhanHangs");

            migrationBuilder.RenameIndex(
                name: "IX_PhanHoiKhachHang_MaKhachHang",
                table: "PhanHoiKhachHangs",
                newName: "IX_PhanHoiKhachHangs_MaKhachHang");

            migrationBuilder.RenameIndex(
                name: "IX_DiaChiNhanHang_MaKhachHang",
                table: "DiaChiNhanHangs",
                newName: "IX_DiaChiNhanHangs_MaKhachHang");

            migrationBuilder.AddColumn<string>(
                name: "QuocTich",
                table: "TacGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TieuSu",
                table: "TacGias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanHoiKhachHangs",
                table: "PhanHoiKhachHangs",
                column: "MaPhanHoiKhachHang");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiaChiNhanHangs",
                table: "DiaChiNhanHangs",
                column: "MaDiaChi");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaChiNhanHangs_KhachHangs_MaKhachHang",
                table: "DiaChiNhanHangs",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_DiaChiNhanHangs_MaDiaChi",
                table: "DonHangs",
                column: "MaDiaChi",
                principalTable: "DiaChiNhanHangs",
                principalColumn: "MaDiaChi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiKhachHangs_KhachHangs_MaKhachHang",
                table: "PhanHoiKhachHangs",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaChiNhanHangs_KhachHangs_MaKhachHang",
                table: "DiaChiNhanHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_DiaChiNhanHangs_MaDiaChi",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiKhachHangs_KhachHangs_MaKhachHang",
                table: "PhanHoiKhachHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanHoiKhachHangs",
                table: "PhanHoiKhachHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiaChiNhanHangs",
                table: "DiaChiNhanHangs");

            migrationBuilder.DropColumn(
                name: "QuocTich",
                table: "TacGias");

            migrationBuilder.DropColumn(
                name: "TieuSu",
                table: "TacGias");

            migrationBuilder.RenameTable(
                name: "PhanHoiKhachHangs",
                newName: "PhanHoiKhachHang");

            migrationBuilder.RenameTable(
                name: "DiaChiNhanHangs",
                newName: "DiaChiNhanHang");

            migrationBuilder.RenameIndex(
                name: "IX_PhanHoiKhachHangs_MaKhachHang",
                table: "PhanHoiKhachHang",
                newName: "IX_PhanHoiKhachHang_MaKhachHang");

            migrationBuilder.RenameIndex(
                name: "IX_DiaChiNhanHangs_MaKhachHang",
                table: "DiaChiNhanHang",
                newName: "IX_DiaChiNhanHang_MaKhachHang");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanHoiKhachHang",
                table: "PhanHoiKhachHang",
                column: "MaPhanHoiKhachHang");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiaChiNhanHang",
                table: "DiaChiNhanHang",
                column: "MaDiaChi");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaChiNhanHang_KhachHangs_MaKhachHang",
                table: "DiaChiNhanHang",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_DiaChiNhanHang_MaDiaChi",
                table: "DonHangs",
                column: "MaDiaChi",
                principalTable: "DiaChiNhanHang",
                principalColumn: "MaDiaChi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiKhachHang_KhachHangs_MaKhachHang",
                table: "PhanHoiKhachHang",
                column: "MaKhachHang",
                principalTable: "KhachHangs",
                principalColumn: "MaKhachHang",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
