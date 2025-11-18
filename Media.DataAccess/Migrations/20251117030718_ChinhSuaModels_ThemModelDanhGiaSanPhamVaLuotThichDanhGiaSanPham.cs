using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaModels_ThemModelDanhGiaSanPhamVaLuotThichDanhGiaSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhGiaSanPhams",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TenHienThi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsAnDanh = table.Column<bool>(type: "bit", nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LuotThich = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaSanPhams", x => x.MaDanhGia);
                    table.ForeignKey(
                        name: "FK_DanhGiaSanPhams_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGiaSanPhams_Saches_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Saches",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LuotThichDanhGiaSanPhams",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuotThichDanhGiaSanPhams", x => new { x.MaKhachHang, x.MaDanhGia });
                    table.ForeignKey(
                        name: "FK_LuotThichDanhGiaSanPhams_DanhGiaSanPhams_MaDanhGia",
                        column: x => x.MaDanhGia,
                        principalTable: "DanhGiaSanPhams",
                        principalColumn: "MaDanhGia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LuotThichDanhGiaSanPhams_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaSanPhams_MaKhachHang",
                table: "DanhGiaSanPhams",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaSanPhams_MaSach",
                table: "DanhGiaSanPhams",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "IX_LuotThichDanhGiaSanPhams_MaDanhGia",
                table: "LuotThichDanhGiaSanPhams",
                column: "MaDanhGia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LuotThichDanhGiaSanPhams");

            migrationBuilder.DropTable(
                name: "DanhGiaSanPhams");
        }
    }
}
