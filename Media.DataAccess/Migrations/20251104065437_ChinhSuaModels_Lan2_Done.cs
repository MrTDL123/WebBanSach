using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaModels_Lan2_Done : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VanChuyens_DonHangs_MaDonHang",
                table: "VanChuyens");

            migrationBuilder.DropIndex(
                name: "IX_VanChuyens_MaDonHang",
                table: "VanChuyens");

            migrationBuilder.DropColumn(
                name: "DiaChiGiaoHang",
                table: "VanChuyens");

            migrationBuilder.DropColumn(
                name: "MaDonHang",
                table: "VanChuyens");

            migrationBuilder.DropColumn(
                name: "PhiVanChuyen",
                table: "DonHangs");

            migrationBuilder.RenameColumn(
                name: "NgayGiaoHang",
                table: "VanChuyens",
                newName: "NgayNhanHangThucTe");

            migrationBuilder.RenameColumn(
                name: "TinhTrangGiaoHang",
                table: "DonHangs",
                newName: "MaVanChuyen");

            migrationBuilder.RenameColumn(
                name: "DiaChiGiaoHang",
                table: "DonHangs",
                newName: "DiaChiChiTiet");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDuKienGiao",
                table: "VanChuyens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaDiaChi",
                table: "DonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoaiNhan",
                table: "DonHangs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiNhan",
                table: "DonHangs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DiaChiNhanHang",
                columns: table => new
                {
                    MaDiaChi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    TinhThanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuanHuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhuongXa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiaChiChiTiet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LaMacDinh = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaChiNhanHang", x => x.MaDiaChi);
                    table.ForeignKey(
                        name: "FK_DiaChiNhanHang_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaDiaChi",
                table: "DonHangs",
                column: "MaDiaChi");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaVanChuyen",
                table: "DonHangs",
                column: "MaVanChuyen");

            migrationBuilder.CreateIndex(
                name: "IX_DiaChiNhanHang_MaKhachHang",
                table: "DiaChiNhanHang",
                column: "MaKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_DiaChiNhanHang_MaDiaChi",
                table: "DonHangs",
                column: "MaDiaChi",
                principalTable: "DiaChiNhanHang",
                principalColumn: "MaDiaChi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_VanChuyens_MaVanChuyen",
                table: "DonHangs",
                column: "MaVanChuyen",
                principalTable: "VanChuyens",
                principalColumn: "MaVanChuyen",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_DiaChiNhanHang_MaDiaChi",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_VanChuyens_MaVanChuyen",
                table: "DonHangs");

            migrationBuilder.DropTable(
                name: "DiaChiNhanHang");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_MaDiaChi",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_MaVanChuyen",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "NgayDuKienGiao",
                table: "VanChuyens");

            migrationBuilder.DropColumn(
                name: "MaDiaChi",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "SoDienThoaiNhan",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "TenNguoiNhan",
                table: "DonHangs");

            migrationBuilder.RenameColumn(
                name: "NgayNhanHangThucTe",
                table: "VanChuyens",
                newName: "NgayGiaoHang");

            migrationBuilder.RenameColumn(
                name: "MaVanChuyen",
                table: "DonHangs",
                newName: "TinhTrangGiaoHang");

            migrationBuilder.RenameColumn(
                name: "DiaChiChiTiet",
                table: "DonHangs",
                newName: "DiaChiGiaoHang");

            migrationBuilder.AddColumn<string>(
                name: "DiaChiGiaoHang",
                table: "VanChuyens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaDonHang",
                table: "VanChuyens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PhiVanChuyen",
                table: "DonHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_VanChuyens_MaDonHang",
                table: "VanChuyens",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VanChuyens_DonHangs_MaDonHang",
                table: "VanChuyens",
                column: "MaDonHang",
                principalTable: "DonHangs",
                principalColumn: "MaDonHang",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
