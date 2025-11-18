using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaModels_CapNhatLaiLogicDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "AspNetRoles",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUsers",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            //        LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChuDes",
            //    columns: table => new
            //    {
            //        MaChuDe = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenChuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        DuongDanURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ParentId = table.Column<int>(type: "int", nullable: true),
            //        ChuDeMaChuDe = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChuDes", x => x.MaChuDe);
            //        table.ForeignKey(
            //            name: "FK_ChuDes_ChuDes_ChuDeMaChuDe",
            //            column: x => x.ChuDeMaChuDe,
            //            principalTable: "ChuDes",
            //            principalColumn: "MaChuDe");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "NhaXuatBans",
            //    columns: table => new
            //    {
            //        MaNhaXuatBan = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenNXB = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_NhaXuatBans", x => x.MaNhaXuatBan);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TacGias",
            //    columns: table => new
            //    {
            //        MaTacGia = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenTG = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TacGias", x => x.MaTacGia);
            //    });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserClaims",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AspNetUserClaims_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserLogins",
            //    columns: table => new
            //    {
            //        LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserLogins_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserRoles",
            //    columns: table => new
            //    {
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "AspNetRoles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AspNetUserRoles_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AspNetUserTokens",
            //    columns: table => new
            //    {
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            //        table.ForeignKey(
            //            name: "FK_AspNetUserTokens_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "KhachHangs",
            //    columns: table => new
            //    {
            //        MaKhachHang = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaTaiKhoan = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        HoTen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        DienThoai = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_KhachHangs", x => x.MaKhachHang);
            //        table.ForeignKey(
            //            name: "FK_KhachHangs_AspNetUsers_MaTaiKhoan",
            //            column: x => x.MaTaiKhoan,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "NhanViens",
            //    columns: table => new
            //    {
            //        MaNhanVien = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaTaiKhoan = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //        DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
            //        NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CCCD = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            //        Luong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            //        BacLuong = table.Column<int>(type: "int", nullable: true),
            //        NgayVaoLam = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        QueQuan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        LoaiNhanVien = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
            //        BoPhanPhuTrach = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CapBac = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_NhanViens", x => x.MaNhanVien);
            //        table.ForeignKey(
            //            name: "FK_NhanViens_AspNetUsers_MaTaiKhoan",
            //            column: x => x.MaTaiKhoan,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "Saches",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhanTramGiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AnhBiaChinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhBiaPhu1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhBiaPhu2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhBiaPhu3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhBiaPhu4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    NhaCungCap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaTacGia = table.Column<int>(type: "int", nullable: false),
                    MaNhaXuatBan = table.Column<int>(type: "int", nullable: false),
                    MaChuDe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saches", x => x.MaSach);
                    table.ForeignKey(
                        name: "FK_Saches_ChuDes_MaChuDe",
                        column: x => x.MaChuDe,
                        principalTable: "ChuDes",
                        principalColumn: "MaChuDe",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Saches_NhaXuatBans_MaNhaXuatBan",
                        column: x => x.MaNhaXuatBan,
                        principalTable: "NhaXuatBans",
                        principalColumn: "MaNhaXuatBan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Saches_TacGias_MaTacGia",
                        column: x => x.MaTacGia,
                        principalTable: "TacGias",
                        principalColumn: "MaTacGia",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    MaTinhThanh = table.Column<int>(type: "int", nullable: true),
                    MaQuanHuyen = table.Column<int>(type: "int", nullable: true),
                    MaPhuongXa = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    MaGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => x.MaGioHang);
                    table.ForeignKey(
                        name: "FK_GioHangs_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "PhanHoiKhachHang",
            //    columns: table => new
            //    {
            //        MaPhanHoiKhachHang = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaKhachHang = table.Column<int>(type: "int", nullable: false),
            //        NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LoaiPhanHoi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PhanHoiKhachHang", x => x.MaPhanHoiKhachHang);
            //        table.ForeignKey(
            //            name: "FK_PhanHoiKhachHang_KhachHangs_MaKhachHang",
            //            column: x => x.MaKhachHang,
            //            principalTable: "KhachHangs",
            //            principalColumn: "MaKhachHang",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChamSocKhachHangs",
            //    columns: table => new
            //    {
            //        MaChamSoc = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaNhanVien = table.Column<int>(type: "int", nullable: false),
            //        MaKhachHang = table.Column<int>(type: "int", nullable: false),
            //        NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        NgayChamSoc = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChamSocKhachHangs", x => x.MaChamSoc);
            //        table.ForeignKey(
            //            name: "FK_ChamSocKhachHangs_KhachHangs_MaKhachHang",
            //            column: x => x.MaKhachHang,
            //            principalTable: "KhachHangs",
            //            principalColumn: "MaKhachHang",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_ChamSocKhachHangs_NhanViens_MaNhanVien",
            //            column: x => x.MaNhanVien,
            //            principalTable: "NhanViens",
            //            principalColumn: "MaNhanVien",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaDiaChi = table.Column<int>(type: "int", nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoaiNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhuongXa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuanHuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TinhThanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiaChiChiTiet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    HinhThucThanhToan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_DonHangs_DiaChiNhanHang_MaDiaChi",
                        column: x => x.MaDiaChi,
                        principalTable: "DiaChiNhanHang",
                        principalColumn: "MaDiaChi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHangs_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietGioHangs",
                columns: table => new
                {
                    MaGioHang = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGioHangs", x => new { x.MaGioHang, x.MaSach });
                    table.ForeignKey(
                        name: "FK_ChiTietGioHangs_GioHangs_MaGioHang",
                        column: x => x.MaGioHang,
                        principalTable: "GioHangs",
                        principalColumn: "MaGioHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietGioHangs_Saches_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Saches",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "ChiTietDonHangs",
            //    columns: table => new
            //    {
            //        MaDonHang = table.Column<int>(type: "int", nullable: false),
            //        MaSach = table.Column<int>(type: "int", nullable: false),
            //        SoLuong = table.Column<int>(type: "int", nullable: false),
            //        DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChiTietDonHangs", x => new { x.MaDonHang, x.MaSach });
            //        table.ForeignKey(
            //            name: "FK_ChiTietDonHangs_DonHangs_MaDonHang",
            //            column: x => x.MaDonHang,
            //            principalTable: "DonHangs",
            //            principalColumn: "MaDonHang",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ChiTietDonHangs_Saches_MaSach",
            //            column: x => x.MaSach,
            //            principalTable: "Saches",
            //            principalColumn: "MaSach",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "HoaDons",
            //    columns: table => new
            //    {
            //        MaHoaDon = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaDonHang = table.Column<int>(type: "int", nullable: false),
            //        NgayLap = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        VAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
            //        table.ForeignKey(
            //            name: "FK_HoaDons_DonHangs_MaDonHang",
            //            column: x => x.MaDonHang,
            //            principalTable: "DonHangs",
            //            principalColumn: "MaDonHang",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PhieuTraHangs",
            //    columns: table => new
            //    {
            //        MaPhieuTraHang = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MaDonHang = table.Column<int>(type: "int", nullable: false),
            //        MaNhanVien = table.Column<int>(type: "int", nullable: false),
            //        MaKhachHang = table.Column<int>(type: "int", nullable: false),
            //        NgayTra = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TrangThai = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PhieuTraHangs", x => x.MaPhieuTraHang);
            //        table.ForeignKey(
            //            name: "FK_PhieuTraHangs_DonHangs_MaDonHang",
            //            column: x => x.MaDonHang,
            //            principalTable: "DonHangs",
            //            principalColumn: "MaDonHang",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PhieuTraHangs_KhachHangs_MaKhachHang",
            //            column: x => x.MaKhachHang,
            //            principalTable: "KhachHangs",
            //            principalColumn: "MaKhachHang",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PhieuTraHangs_NhanViens_MaNhanVien",
            //            column: x => x.MaNhanVien,
            //            principalTable: "NhanViens",
            //            principalColumn: "MaNhanVien",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "VanChuyens",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    DonViVanChuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaVanDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhiVanChuyen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiGiaoHang = table.Column<int>(type: "int", nullable: false),
                    NgayDuKienGiao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayNhanHangThucTe = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VanChuyens", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_VanChuyens_DonHangs_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHangs",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietTraHangs",
                columns: table => new
                {
                    MaPhieuTraHang = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    SoLuongTra = table.Column<int>(type: "int", nullable: false),
                    DonGiaHoan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTienHoan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LyDoTraHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietTraHangs", x => new { x.MaPhieuTraHang, x.MaSach });
                    table.ForeignKey(
                        name: "FK_ChiTietTraHangs_PhieuTraHangs_MaPhieuTraHang",
                        column: x => x.MaPhieuTraHang,
                        principalTable: "PhieuTraHangs",
                        principalColumn: "MaPhieuTraHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietTraHangs_Saches_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Saches",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                });

        //    migrationBuilder.CreateIndex(
        //        name: "IX_AspNetRoleClaims_RoleId",
        //        table: "AspNetRoleClaims",
        //        column: "RoleId");

        //    migrationBuilder.CreateIndex(
        //        name: "RoleNameIndex",
        //        table: "AspNetRoles",
        //        column: "NormalizedName",
        //        unique: true,
        //        filter: "[NormalizedName] IS NOT NULL");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_AspNetUserClaims_UserId",
        //        table: "AspNetUserClaims",
        //        column: "UserId");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_AspNetUserLogins_UserId",
        //        table: "AspNetUserLogins",
        //        column: "UserId");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_AspNetUserRoles_RoleId",
        //        table: "AspNetUserRoles",
        //        column: "RoleId");

        //    migrationBuilder.CreateIndex(
        //        name: "EmailIndex",
        //        table: "AspNetUsers",
        //        column: "NormalizedEmail");

        //    migrationBuilder.CreateIndex(
        //        name: "UserNameIndex",
        //        table: "AspNetUsers",
        //        column: "NormalizedUserName",
        //        unique: true,
        //        filter: "[NormalizedUserName] IS NOT NULL");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChamSocKhachHangs_MaKhachHang",
        //        table: "ChamSocKhachHangs",
        //        column: "MaKhachHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChamSocKhachHangs_MaNhanVien",
        //        table: "ChamSocKhachHangs",
        //        column: "MaNhanVien");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChiTietDonHangs_MaSach",
        //        table: "ChiTietDonHangs",
        //        column: "MaSach");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChiTietGioHangs_MaSach",
        //        table: "ChiTietGioHangs",
        //        column: "MaSach");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChiTietTraHangs_MaSach",
        //        table: "ChiTietTraHangs",
        //        column: "MaSach");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_ChuDes_ChuDeMaChuDe",
        //        table: "ChuDes",
        //        column: "ChuDeMaChuDe");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_DiaChiNhanHang_MaKhachHang",
        //        table: "DiaChiNhanHang",
        //        column: "MaKhachHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_DonHangs_MaDiaChi",
        //        table: "DonHangs",
        //        column: "MaDiaChi");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_DonHangs_MaKhachHang",
        //        table: "DonHangs",
        //        column: "MaKhachHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_DonHangs_MaNhanVien",
        //        table: "DonHangs",
        //        column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_MaKhachHang",
                table: "GioHangs",
                column: "MaKhachHang",
                unique: true);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_HoaDons_MaDonHang",
        //        table: "HoaDons",
        //        column: "MaDonHang",
        //        unique: true);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_KhachHangs_MaTaiKhoan",
        //        table: "KhachHangs",
        //        column: "MaTaiKhoan",
        //        unique: true);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_NhanViens_MaTaiKhoan",
        //        table: "NhanViens",
        //        column: "MaTaiKhoan",
        //        unique: true);

        //    migrationBuilder.CreateIndex(
        //        name: "IX_PhanHoiKhachHang_MaKhachHang",
        //        table: "PhanHoiKhachHang",
        //        column: "MaKhachHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_PhieuTraHangs_MaDonHang",
        //        table: "PhieuTraHangs",
        //        column: "MaDonHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_PhieuTraHangs_MaKhachHang",
        //        table: "PhieuTraHangs",
        //        column: "MaKhachHang");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_PhieuTraHangs_MaNhanVien",
        //        table: "PhieuTraHangs",
        //        column: "MaNhanVien");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_Saches_MaChuDe",
        //        table: "Saches",
        //        column: "MaChuDe");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_Saches_MaNhaXuatBan",
        //        table: "Saches",
        //        column: "MaNhaXuatBan");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_Saches_MaTacGia",
        //        table: "Saches",
        //        column: "MaTacGia");
        //}

        ///// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropTable(
        //        name: "AspNetRoleClaims");

        //    migrationBuilder.DropTable(
        //        name: "AspNetUserClaims");

        //    migrationBuilder.DropTable(
        //        name: "AspNetUserLogins");

        //    migrationBuilder.DropTable(
        //        name: "AspNetUserRoles");

        //    migrationBuilder.DropTable(
        //        name: "AspNetUserTokens");

        //    migrationBuilder.DropTable(
        //        name: "ChamSocKhachHangs");

        //    migrationBuilder.DropTable(
        //        name: "ChiTietDonHangs");

        //    migrationBuilder.DropTable(
        //        name: "ChiTietGioHangs");

        //    migrationBuilder.DropTable(
        //        name: "ChiTietTraHangs");

        //    migrationBuilder.DropTable(
        //        name: "HoaDons");

        //    migrationBuilder.DropTable(
        //        name: "PhanHoiKhachHang");

            migrationBuilder.DropTable(
                name: "VanChuyens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

        //    migrationBuilder.DropTable(
        //        name: "GioHangs");

        //    migrationBuilder.DropTable(
        //        name: "PhieuTraHangs");

        //    migrationBuilder.DropTable(
        //        name: "Saches");

        //    migrationBuilder.DropTable(
        //        name: "DonHangs");

        //    migrationBuilder.DropTable(
        //        name: "ChuDes");

        //    migrationBuilder.DropTable(
        //        name: "NhaXuatBans");

        //    migrationBuilder.DropTable(
        //        name: "TacGias");

        //    migrationBuilder.DropTable(
        //        name: "DiaChiNhanHang");

        //    migrationBuilder.DropTable(
        //        name: "NhanViens");

            migrationBuilder.DropTable(
                name: "KhachHangs");

        //    migrationBuilder.DropTable(
        //        name: "AspNetUsers");
        }
    }
}
