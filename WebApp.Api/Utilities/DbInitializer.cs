using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Entities;

namespace WebApp.Api.Utilities
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // 1. Seed Roles nếu chưa có
            string[] roles = ["Admin", "Customer", "Staff"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role { Name = role, Description = $"Role {role}" });
                }
            }

            // 2. Seed Default User (Admin & Customer) nếu chưa có
            var defaultEmail = "jackacevn@gmail.com";
            var existingUser = await userManager.FindByEmailAsync(defaultEmail);
            if (existingUser == null)
            {
                var newUser = User.CreateCustomer(
                    defaultEmail,
                    fullName: "Nguyễn Việt Tiến",
                    phoneNumber: "0896699703"
                );
                newUser.NormalizedUserName = defaultEmail.ToUpper();
                newUser.NormalizedEmail = defaultEmail.ToUpper();
                newUser.EmailConfirmed = true;
                newUser.PhoneNumberConfirmed = true;
                newUser.UpdateProfile("Nguyễn Việt Tiến", "Gia Lai", new DateTime(2005, 12, 8));

                var createRes = await userManager.CreateAsync(newUser, "12345");
                if (createRes.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                    await userManager.AddToRoleAsync(newUser, "Customer");
                }
            }

            // 3. Seed Banners (3 Banners văn hóa truyền thống)
            if (!await context.Banners.AnyAsync())
            {
                var banners = new List<Banner>
                {
                    new()
                    {
                        Title = "Tinh Hoa Văn Hóa & Lịch Sử Việt Nam",
                        ImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=1200&q=80",
                        TargetUrl = "/collections/van-hoa-viet-nam",
                        DisplayOrder = 1,
                        IsActive = true
                    },
                    new()
                    {
                        Title = "Đại Việt Sử Ký Toàn Thư - Bản Khắc Gỗ Cổ Truyền",
                        ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=1200&q=80",
                        TargetUrl = "/collections/lich-su-co-trung-dai",
                        DisplayOrder = 2,
                        IsActive = true
                    },
                    new()
                    {
                        Title = "Tủ Sách 0 Đồng - Lan Tỏa Tri Thức Cộng Đồng",
                        ImageUrl = "https://images.unsplash.com/photo-1497633762265-9d179a990aa6?auto=format&fit=crop&w=1200&q=80",
                        TargetUrl = "/free-books",
                        DisplayOrder = 3,
                        IsActive = true
                    }
                };
                await context.Banners.AddRangeAsync(banners);
                await context.SaveChangesAsync();
            }

            // 4. Seed Categories (5 Danh mục sách văn hóa)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { CategoryName = "Văn Hóa - Phong Tục Việt", Description = "Khảo cứu phong tục, lễ hội và bản sắc văn hóa dân tộc Việt Nam", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { CategoryName = "Lịch Sử & Dã Sử", Description = "Sử ký, biên niên sử và các triều đại lịch sử Việt Nam", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { CategoryName = "Văn Học Cổ Điển & Dân Gian", Description = "Thơ ca, truyện cổ tích, ca dao, tục ngữ và tác phẩm kinh điển", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { CategoryName = "Kiến Trúc & Nghệ Thuật Truyền Thống", Description = "Mỹ thuật đình làng, gốm sứ cổ, kiến trúc kinh kỳ và di sản", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { CategoryName = "Triết Học Đông Phương & Tư Tưởng", Description = "Nho giáo, Phật giáo, Đạo giáo và tư tưởng Việt Nam qua các thời kỳ", IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 5. Seed Authors (5 Tác giả)
            if (!await context.Authors.AnyAsync())
            {
                var authors = new List<Author>
                {
                    new() { AuthorName = "Toan Ánh", Biography = "Nhà văn hóa, nhà khảo cứu phong tục tập quán cổ truyền Việt Nam lỗi lạc", IsActive = true },
                    new() { AuthorName = "Ngô Sĩ Liên", Biography = "Nhà sử học trứ danh thời Hậu Lê, đồng tác giả Đại Việt Sử Ký Toàn Thư", IsActive = true },
                    new() { AuthorName = "Nguyễn Du", Biography = "Đại thi hào dân tộc Việt Nam, Danh nhân văn hóa thế giới, tác giả Truyện Kiều", IsActive = true },
                    new() { AuthorName = "Trần Trọng Kim", Biography = "Học giả, nhà sử học và giáo dục lớn, tác giả Việt Nam Sử Lược", IsActive = true },
                    new() { AuthorName = "Sơn Nam", Biography = "Nhà văn, nhà nghiên cứu văn hóa phong thổ và con người Nam Bộ", IsActive = true }
                };
                await context.Authors.AddRangeAsync(authors);
                await context.SaveChangesAsync();
            }

            // 6. Seed Publishers (2 Nhà xuất bản)
            if (!await context.Publishers.AnyAsync())
            {
                var publishers = new List<Publisher>
                {
                    new() { PublisherName = "NXB Kim Đồng", Address = "55 Quang Trung, Hoàn Kiếm, Hà Nội", PhoneNumber = "02439434730", IsActive = true },
                    new() { PublisherName = "NXB Văn Học", Address = "18 Nguyễn Trường Tộ, Ba Đình, Hà Nội", PhoneNumber = "02437161518", IsActive = true }
                };
                await context.Publishers.AddRangeAsync(publishers);
                await context.SaveChangesAsync();
            }

            // 7. Seed Products (12 Sách văn hóa truyền thống)
            if (!await context.Products.AnyAsync())
            {
                var cats = await context.Categories.OrderBy(c => c.CategoryId).ToListAsync();
                var authors = await context.Authors.OrderBy(a => a.AuthorId).ToListAsync();
                var publishers = await context.Publishers.OrderBy(p => p.PublisherId).ToListAsync();

                var cPhongTuc = cats[0].CategoryId;
                var cLichSu = cats.Count > 1 ? cats[1].CategoryId : cats[0].CategoryId;
                var cVanHoc = cats.Count > 2 ? cats[2].CategoryId : cats[0].CategoryId;
                var cNgheThuat = cats.Count > 3 ? cats[3].CategoryId : cats[0].CategoryId;
                var cTrietHoc = cats.Count > 4 ? cats[4].CategoryId : cats[0].CategoryId;

                var aToanAnh = authors[0].CategoryIdAuthor(authors, 0);
                var aNgoSiLien = authors[0].CategoryIdAuthor(authors, 1);
                var aNguyenDu = authors[0].CategoryIdAuthor(authors, 2);
                var aTranTrongKim = authors[0].CategoryIdAuthor(authors, 3);
                var aSonNam = authors[0].CategoryIdAuthor(authors, 4);

                var pKimDong = publishers[0].PublisherId;
                var pVanHoc = publishers.Count > 1 ? publishers[1].PublisherId : publishers[0].PublisherId;

                var products = new List<Product>
                {
                    new()
                    {
                        Title = "Nếp Cũ - Hội Hè Đình Đám",
                        Description = "Khảo cứu cặn kẽ về các lễ hội dân gian, tế lễ, hội làng truyền thống của người Việt xưa.",
                        Price = 135000m,
                        DiscountPercent = 10,
                        StockQuantity = 50,
                        MainImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "NXB Trẻ",
                        CategoryId = cPhongTuc,
                        AuthorId = aToanAnh,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Nếp Cũ - Tín Ngưỡng Việt Nam",
                        Description = "Bức tranh toàn cảnh về tín ngưỡng thành hoàng, thờ cúng tổ tiên và phong tục dân tộc.",
                        Price = 145000m,
                        DiscountPercent = 15,
                        StockQuantity = 45,
                        MainImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "NXB Trẻ",
                        CategoryId = cPhongTuc,
                        AuthorId = aToanAnh,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Đại Việt Sử Ký Toàn Thư (Trọn Bộ Bìa Cứng)",
                        Description = "Bộ quốc sử vĩ đại nhất ghi chép từ thời Hồng Bàng đến thời kỳ Hậu Lê sơ.",
                        Price = 450000m,
                        DiscountPercent = 20,
                        StockQuantity = 30,
                        MainImageUrl = "https://images.unsplash.com/photo-1532012164546-f432f2e3777f?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Đông A Books",
                        CategoryId = cLichSu,
                        AuthorId = aNgoSiLien,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Việt Nam Sử Lược",
                        Description = "Quyển thông sử Việt Nam đầu tiên viết bằng chữ quốc ngữ với góc nhìn khoa học, khúc chiết.",
                        Price = 160000m,
                        DiscountPercent = 10,
                        StockQuantity = 60,
                        MainImageUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Nhã Nam",
                        CategoryId = cLichSu,
                        AuthorId = aTranTrongKim,
                        PublisherId = pKimDong,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Truyện Kiều - Ấn Bản Điêu Khắc Mỹ Thuật",
                        Description = "Kiệt tác bất hủ của đại thi hào Nguyễn Du, in kèm tranh minh họa cổ truyền của các họa sĩ Đông Dương.",
                        Price = 280000m,
                        DiscountPercent = 15,
                        StockQuantity = 40,
                        MainImageUrl = "https://images.unsplash.com/photo-1516979187457-637abb4f9353?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Nhã Nam",
                        CategoryId = cVanHoc,
                        AuthorId = aNguyenDu,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Hương Rừng Cà Mau",
                        Description = "Tập truyện ngắn kinh điển tái hiện công cuộc khai phá miền đất phương Nam kiêu hùng và phóng khoáng.",
                        Price = 110000m,
                        DiscountPercent = 5,
                        StockQuantity = 35,
                        MainImageUrl = "https://images.unsplash.com/photo-1476275466078-4007374efbbe?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "NXB Trẻ",
                        CategoryId = cVanHoc,
                        AuthorId = aSonNam,
                        PublisherId = pKimDong,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Văn Minh Miệt Vườn",
                        Description = "Ghi chép sâu sắc về nếp sống, tính cách và văn hóa sinh hoạt dân dã của cư dân đồng bằng sông Cửu Long.",
                        Price = 95000m,
                        DiscountPercent = 0,
                        StockQuantity = 50,
                        MainImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "NXB Trẻ",
                        CategoryId = cPhongTuc,
                        AuthorId = aSonNam,
                        PublisherId = pKimDong,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Mỹ Thuật Đình Làng Việt Nam",
                        Description = "Nghiên cứu kiến trúc, điêu khắc gỗ chạm và nghệ thuật dân gian tại các ngôi đình làng Bắc Bộ.",
                        Price = 320000m,
                        DiscountPercent = 10,
                        StockQuantity = 25,
                        MainImageUrl = "https://images.unsplash.com/photo-1497633762265-9d179a990aa6?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Mỹ Thuật Books",
                        CategoryId = cNgheThuat,
                        AuthorId = aToanAnh,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Gốm Cổ Truyền Thống Việt Nam",
                        Description = "Lịch sử và tinh hoa nghệ thuật gốm Chu Đậu, Bát Tràng từ thời Lý - Trần đến nay.",
                        Price = 250000m,
                        DiscountPercent = 12,
                        StockQuantity = 20,
                        MainImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Mỹ Thuật Books",
                        CategoryId = cNgheThuat,
                        AuthorId = aToanAnh,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Nho Giáo Việt Nam",
                        Description = "Khảo luận quá trình du nhập, tiếp biến và ảnh hưởng sâu rộng của đạo Nho trong lịch sử tư tưởng Việt.",
                        Price = 175000m,
                        DiscountPercent = 10,
                        StockQuantity = 35,
                        MainImageUrl = "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Tri Thức Books",
                        CategoryId = cTrietHoc,
                        AuthorId = aTranTrongKim,
                        PublisherId = pKimDong,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Phật Giáo Trong Văn Hóa Việt Nam",
                        Description = "Ảnh hưởng của Thiền phái Trúc Lâm Yên Tử và tư tưởng Phật giáo thời Lý - Trần đối với quốc gia.",
                        Price = 190000m,
                        DiscountPercent = 15,
                        StockQuantity = 30,
                        MainImageUrl = "https://images.unsplash.com/photo-1506880018603-83d5b814b5a6?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "Tôn Giáo Books",
                        CategoryId = cTrietHoc,
                        AuthorId = aTranTrongKim,
                        PublisherId = pVanHoc,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Kho Tàng Truyện Cổ Tích Việt Nam",
                        Description = "Bộ sưu tầm công phu và đầy đủ nhất các câu chuyện cổ tích thấm đẫm tâm hồn và nhân sinh quan người Việt.",
                        Price = 350000m,
                        DiscountPercent = 20,
                        StockQuantity = 40,
                        MainImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=600&q=80",
                        SupplierName = "NXB Kim Đồng",
                        CategoryId = cVanHoc,
                        AuthorId = aTranTrongKim,
                        PublisherId = pKimDong,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            // 8. Seed FreeBooks (3 Tủ sách 0 đồng)
            if (!await context.FreeBooks.AnyAsync())
            {
                var freeBooks = new List<FreeBook>
                {
                    new()
                    {
                        Title = "Lược Khảo Ca Dao Tục Ngữ Dân Gian",
                        AuthorName = "Nhiều Tác Giả",
                        Description = "Sách tặng cộng đồng học sinh, sinh viên yêu thích văn hóa dân gian Việt Nam.",
                        CoverImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?auto=format&fit=crop&w=600&q=80",
                        AvailableQty = 20,
                        TotalDonatedQty = 20,
                        TotalRequestedQty = 0,
                        Condition = "Như mới (99%)",
                        Source = "Quỹ Khuyến Học Văn Hóa",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Danh Nhân Đất Việt - Những Tấm Gương Sáng",
                        AuthorName = "Hội Khoa Học Lịch Sử",
                        Description = "Tuyển tập tiểu sử và chiến công của các anh hùng hào kiệt trong lịch sử dựng nước và giữ nước.",
                        CoverImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=600&q=80",
                        AvailableQty = 15,
                        TotalDonatedQty = 15,
                        TotalRequestedQty = 0,
                        Condition = "Rất tốt (95%)",
                        Source = "Đoàn Thanh Niên trao tặng",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Title = "Cổ Tích Việt Nam Dành Cho Thiếu Nhi",
                        AuthorName = "Nguyễn Đổng Chi",
                        Description = "Sách truyện tranh và chữ minh họa màu dành tặng trẻ em hiếu học vùng xa.",
                        CoverImageUrl = "https://images.unsplash.com/photo-1497633762265-9d179a990aa6?auto=format&fit=crop&w=600&q=80",
                        AvailableQty = 30,
                        TotalDonatedQty = 30,
                        TotalRequestedQty = 0,
                        Condition = "Mới 100%",
                        Source = "Tủ Sách Hạt Giống Tâm Hồn",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                await context.FreeBooks.AddRangeAsync(freeBooks);
                await context.SaveChangesAsync();
            }

            // 9. Seed Coupons (2 mã giảm giá)
            if (!await context.Coupons.AnyAsync())
            {
                var coupons = new List<Coupon>
                {
                    new()
                    {
                        Code = "VANHOA10",
                        DiscountAmount = 25000m, // Giảm 25.000đ tương ứng ~10% cho đơn từ 200.000đ
                        MinOrderAmount = 200000m,
                        StartDate = DateTime.UtcNow.AddDays(-7),
                        EndDate = DateTime.UtcNow.AddYears(1),
                        UsageLimit = 200,
                        TimesUsed = 0,
                        IsActive = true
                    },
                    new()
                    {
                        Code = "GIAM30K",
                        DiscountAmount = 30000m,
                        MinOrderAmount = 150000m,
                        StartDate = DateTime.UtcNow.AddDays(-7),
                        EndDate = DateTime.UtcNow.AddYears(1),
                        UsageLimit = 150,
                        TimesUsed = 0,
                        IsActive = true
                    }
                };
                await context.Coupons.AddRangeAsync(coupons);
                await context.SaveChangesAsync();
            }
        }

        private static int CategoryIdAuthor(this Author fallback, List<Author> list, int index)
        {
            return index < list.Count ? list[index].AuthorId : fallback.AuthorId;
        }
    }
}
