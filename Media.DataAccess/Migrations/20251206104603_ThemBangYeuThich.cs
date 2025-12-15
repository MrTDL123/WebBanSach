using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ThemBangYeuThich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YeuThich",
                columns: table => new
                {
                    MaYeuThich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuThich", x => x.MaYeuThich);
                    table.ForeignKey(
                        name: "FK_YeuThich_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YeuThich_Saches_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Saches",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YeuThich_MaKhachHang",
                table: "YeuThich",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_YeuThich_MaSach",
                table: "YeuThich",
                column: "MaSach");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YeuThich");
        }
    }
}
