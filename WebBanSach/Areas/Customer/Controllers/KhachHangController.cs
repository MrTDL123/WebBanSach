using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Media.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
// --- 1. THÊM CÁC USING NÀY ---
using Microsoft.AspNetCore.Identity.UI.Services; // Để sử dụng IEmailSender
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics; // Để Debug.WriteLine
using System.Net.Mail; // Để bắt SmtpException
using System.Security.Claims;
using System.Threading.Tasks;

namespace Media.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class KhachHangController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IGioHangService _gioHangService;
        private readonly LocationService _locationService;

        public KhachHangController(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager, IUnitOfWork unit, IEmailSender emailSender, IGioHangService gioHangService, LocationService locationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unit = unit;
            _emailSender = emailSender; // Gán giá trị
            _gioHangService = gioHangService;
            _locationService = locationService;
        }

        [HttpGet]
        public IActionResult DangKy() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyVM model)
        {
            // (Code Đăng ký POST của bạn giữ nguyên,
            // nó đã tự kiểm tra MaOTP từ session rồi)

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
            }

            var existingPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.SoDienThoai);
            if (existingPhone != null)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var otpSession = HttpContext.Session.GetString("OTP_" + model.SoDienThoai);
            if (otpSession == null || otpSession != model.MaOTP)
            {
                ModelState.AddModelError("MaOTP", "Mã OTP không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            var user = new TaiKhoan
            {
                UserName = model.Email, // Nên dùng Email làm UserName vì nó duy nhất
                Email = model.Email,
                PhoneNumber = model.SoDienThoai,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.MatKhau);
            if (result.Succeeded)
            {
                var khachHang = new KhachHang
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    DienThoai = model.SoDienThoai,
                    DiaChi = model.DiaChi,
                    MaTaiKhoan = user.Id
                };

                _unit.KhachHangs.Add(khachHang);
                await _unit.SaveAsync();

                await _userManager.AddClaimAsync(user, new Claim("HoTen", khachHang.HoTen));
                // ⭐ KẾT THÚC SỬA LỖI

                // ... (Code đăng nhập người dùng ngay sau khi đăng ký)
                await _signInManager.SignInAsync(user, isPersistent: false);

                await _userManager.AddToRoleAsync(user, SD.Role_Customer);

                // ✅ Gửi email thông báo đăng ký thành công
                string subject = "Chúc mừng bạn đã đăng ký thành công - Nhà Sách Văn Hóa Truyền Thống";

                string body = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Đăng ký tài khoản thành công</title>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #f5f6f7;
                                margin: 0;
                                padding: 0;
                            }}
                            .email-container {{
                                max-width: 650px;
                                margin: 40px auto;
                                background: #ffffff;
                                border-radius: 12px;
                                box-shadow: 0 6px 18px rgba(0, 0, 0, 0.08);
                                overflow: hidden;
                            }}
                            .email-header {{
                                background: linear-gradient(135deg, #d70018, #f0717f);
                                color: #fff;
                                text-align: center;
                                padding: 25px 30px;
                            }}
                            .email-header h1 {{
                                margin: 0;
                                font-size: 26px;
                                font-weight: 700;
                            }}
                            .email-body {{
                                padding: 35px 40px;
                                color: #333;
                                line-height: 1.8;
                            }}
                            .email-body h2 {{
                                color: #d70018;
                                font-size: 22px;
                                margin-bottom: 10px;
                            }}
                            .email-body p {{
                                margin: 10px 0;
                                font-size: 15px;
                            }}
                            .btn-login {{
                                display: inline-block;
                                background: #d70018;
                                color: #fff !important;
                                padding: 10px 22px;
                                border-radius: 6px;
                                text-decoration: none;
                                margin-top: 20px;
                                font-weight: 600;
                            }}
                            .email-footer {{
                                background: #fafafa;
                                text-align: center;
                                padding: 20px 15px;
                                color: #777;
                                font-size: 13px;
                                border-top: 1px solid #eee;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>
                                <h1>Nhà Sách Văn Hóa Truyền Thống</h1>
                            </div>
                            <div class='email-body'>
                                <h2>Xin chào {model.HoTen},</h2>
                                <p>Chúc mừng bạn đã đăng ký tài khoản thành công tại <strong>Nhà Sách Văn Hóa Truyền Thống</strong>.</p>
                                <p>Từ giờ, bạn có thể đăng nhập để:</p>
                                <ul style='margin: 10px 0 20px 20px;'>
                                    <li>Mua sắm hàng ngàn đầu sách đa dạng.</li>
                                    <li>Theo dõi đơn hàng và lịch sử mua hàng.</li>
                                    <li>Nhận ưu đãi và khuyến mãi độc quyền.</li>
                                </ul>
                                <p style='text-align:center;'>
                                    <a href='https://nhasachvanhoatruyenthong.vn/dangnhap' class='btn-login'>Đăng nhập ngay</a>
                                </p>
                                <p>Cảm ơn bạn đã tin tưởng lựa chọn chúng tôi!</p>
                            </div>
                            <div class='email-footer'>
                                © {DateTime.Now.Year} Nhà Sách Văn Hóa Truyền Thống.  
                                <br />Nơi lưu giữ tri thức và giá trị văn hóa Việt.
                            </div>
                        </div>
                    </body>
                    </html>";


                try
                {
                    await _emailSender.SendEmailAsync(model.Email, subject, body);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Không gửi được email xác nhận: " + ex.Message);
                }

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("DangNhap", "KhachHang");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult DangNhap() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapVM model)
        {
            // (Code Đăng nhập giữ nguyên)
            if (!ModelState.IsValid)
                return View(model);

            TaiKhoan user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Tài khoản không tồn tại.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.MatKhau,
                model.GhiNhoDangNhap,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == user.Id);

                // 2. Xóa claim "HoTen" cũ (nếu có) để phòng trường hợp người dùng đổi tên
                var oldClaims = await _userManager.GetClaimsAsync(user);
                var oldHoTenClaim = oldClaims.FirstOrDefault(c => c.Type == "HoTen");
                if (oldHoTenClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, oldHoTenClaim);
                }

                // 3. Thêm claim "HoTen" mới
                if (khachHang != null)
                {
                    await _userManager.AddClaimAsync(user, new Claim("HoTen", khachHang.HoTen));
                }

                // 4. (Rất quan trọng) Làm mới cookie đăng nhập để nhận claim mới
                await _signInManager.RefreshSignInAsync(user);

                _gioHangService.GopGioHangSessionVaoDb(user.Id);
                if (TempData["ReturnUrl"] != null)
                {
                    return Redirect(TempData["ReturnUrl"].ToString());
                }

                return RedirectToAction("TrangChu", "Home", new { area = "Customer" });
            }

            ModelState.AddModelError("MatKhau", "Mật khẩu không đúng.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GuiOTP(string soDienThoai, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập email." });
            }

            try
            {
                // 1. Tạo mã OTP
                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                // 2. Lưu OTP vào Session (gắn với SĐT)
                HttpContext.Session.SetString("OTP_" + soDienThoai, otp);

                // 3. Chuẩn bị nội dung email
                string subject = "Mã xác thực OTP - Nhà Sách Văn Hóa Truyền Thống";

                string emailBody = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Mã xác thực OTP</title>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #f5f5f5;
                                margin: 0;
                                padding: 0;
                            }}
                            .email-container {{
                                max-width: 600px;
                                background: #ffffff;
                                margin: 40px auto;
                                border-radius: 12px;
                                overflow: hidden;
                                box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
                            }}
                            .email-header {{
                                background: linear-gradient(135deg, #d70018, #f0717f);
                                color: #ffffff;
                                padding: 20px 30px;
                                text-align: center;
                            }}
                            .email-header h1 {{
                                margin: 0;
                                font-size: 24px;
                                letter-spacing: 0.5px;
                            }}
                            .email-body {{
                                padding: 30px;
                                color: #333;
                                line-height: 1.7;
                            }}
                            .otp-box {{
                                text-align: center;
                                margin: 25px 0;
                            }}
                            .otp-code {{
                                display: inline-block;
                                background: #f8f9fa;
                                color: #d70018;
                                font-weight: bold;
                                font-size: 32px;
                                letter-spacing: 4px;
                                border: 2px dashed #f0717f;
                                border-radius: 8px;
                                padding: 15px 25px;
                            }}
                            .email-footer {{
                                text-align: center;
                                font-size: 13px;
                                color: #888;
                                background: #fafafa;
                                padding: 15px 20px;
                                border-top: 1px solid #eee;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>
                                <h1>Nhà Sách Văn Hóa Truyền Thống</h1>
                            </div>
                            <div class='email-body'>
                                <p>Xin chào,</p>
                                <p>Bạn đang thực hiện <strong>xác thực tài khoản</strong> cho số điện thoại <strong>{soDienThoai}</strong>.</p>
                                <p>Vui lòng nhập mã OTP dưới đây để hoàn tất quá trình đăng ký:</p>
                                <div class='otp-box'>
                                    <div class='otp-code'>{otp}</div>
                                </div>
                                <p><strong>Lưu ý:</strong> Mã OTP có hiệu lực trong 5 phút. 
                                Không chia sẻ mã này cho bất kỳ ai để đảm bảo an toàn tài khoản của bạn.</p>
                            </div>
                            <div class='email-footer'>
                                © {DateTime.Now.Year} Nhà Sách Văn Hóa Truyền Thống — Nơi lưu giữ tri thức Việt.
                            </div>
                        </div>
                    </body>
                    </html>";

                // 4. GỌI DỊCH VỤ EMAIL SENDER CỦA BẠN
                await _emailSender.SendEmailAsync(email, subject, emailBody);

                // 5. Trả về thành công
                return Json(new
                {
                    success = true,
                    message = $"Một mã OTP đã được gửi đến email: {email}. Vui lòng kiểm tra email (kể cả Spam) để lấy mã."
                });
            }
            catch (SmtpException smtpEx) // Bắt lỗi nếu email KHÔNG TỒN TẠI (MailKit cũng ném lỗi này)
            {
                Debug.WriteLine("Gửi email thất bại: " + smtpEx.Message);
                return Json(new
                {
                    success = false,
                    message = "Không thể gửi OTP. Email không hợp lệ hoặc có lỗi. Vui lòng kiểm tra lại email."
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi GuiOTP: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Có lỗi hệ thống, không thể gửi OTP. Vui lòng thử lại."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GuiOTPQuenMatKhau(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập email." });
            }

            try
            {
                // 1. Tạo mã OTP
                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                // 2. Lưu OTP vào Session (gắn với SĐT)
                HttpContext.Session.SetString("OTP_" + email, otp);

                // 3. Chuẩn bị nội dung email
                string subject = "Mã xác thực OTP - Nhà Sách Văn Hóa Truyền Thống";

                string emailBody = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Mã xác thực OTP</title>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #f5f5f5;
                                margin: 0;
                                padding: 0;
                            }}
                            .email-container {{
                                max-width: 600px;
                                background: #ffffff;
                                margin: 40px auto;
                                border-radius: 12px;
                                overflow: hidden;
                                box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
                            }}
                            .email-header {{
                                background: linear-gradient(135deg, #d70018, #f0717f);
                                color: #ffffff;
                                padding: 20px 30px;
                                text-align: center;
                            }}
                            .email-header h1 {{
                                margin: 0;
                                font-size: 24px;
                                letter-spacing: 0.5px;
                            }}
                            .email-body {{
                                padding: 30px;
                                color: #333;
                                line-height: 1.7;
                            }}
                            .otp-box {{
                                text-align: center;
                                margin: 25px 0;
                            }}
                            .otp-code {{
                                display: inline-block;
                                background: #f8f9fa;
                                color: #d70018;
                                font-weight: bold;
                                font-size: 32px;
                                letter-spacing: 4px;
                                border: 2px dashed #f0717f;
                                border-radius: 8px;
                                padding: 15px 25px;
                            }}
                            .email-footer {{
                                text-align: center;
                                font-size: 13px;
                                color: #888;
                                background: #fafafa;
                                padding: 15px 20px;
                                border-top: 1px solid #eee;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>
                                <h1>Nhà Sách Văn Hóa Truyền Thống</h1>
                            </div>
                            <div class='email-body'>
                                <p>Xin chào,</p>
                                <p>Bạn đang thực hiện <strong>xác thực tài khoản</strong> cho email <strong>{email}</strong>.</p>
                                <p>Vui lòng nhập mã OTP dưới đây để hoàn tất quá trình xác thực email để thay đổi mật khẩu:</p>
                                <div class='otp-box'>
                                    <div class='otp-code'>{otp}</div>
                                </div>
                                <p><strong>Lưu ý:</strong> Mã OTP có hiệu lực trong 5 phút. 
                                Không chia sẻ mã này cho bất kỳ ai để đảm bảo an toàn tài khoản của bạn.</p>
                            </div>
                            <div class='email-footer'>
                                © {DateTime.Now.Year} Nhà Sách Văn Hóa Truyền Thống — Nơi lưu giữ tri thức Việt.
                            </div>
                        </div>
                    </body>
                    </html>";

                // 4. GỌI DỊCH VỤ EMAIL SENDER CỦA BẠN
                await _emailSender.SendEmailAsync(email, subject, emailBody);

                // 5. Trả về thành công
                return Json(new
                {
                    success = true,
                    message = $"Một mã OTP đã được gửi đến email: {email}. Vui lòng kiểm tra email (kể cả Spam) để lấy mã."
                });
            }
            catch (SmtpException smtpEx) // Bắt lỗi nếu email KHÔNG TỒN TẠI (MailKit cũng ném lỗi này)
            {
                Debug.WriteLine("Gửi email thất bại: " + smtpEx.Message);
                return Json(new
                {
                    success = false,
                    message = "Không thể gửi OTP. Email không hợp lệ hoặc có lỗi. Vui lòng kiểm tra lại email."
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi GuiOTP: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Có lỗi hệ thống, không thể gửi OTP. Vui lòng thử lại."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("TrangChu", "Home");
        }

        [HttpGet]
        public IActionResult QuenMatKhau() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Xác thực OTP
            var otpSession = HttpContext.Session.GetString("OTP_" + model.Email);
            if (otpSession == null || otpSession != model.MaOTP)
            {
                ModelState.AddModelError("MaOTP", "Mã OTP không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            // Tìm tài khoản theo email hoặc số điện thoại
            TaiKhoan user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản.");
                return View(model);
            }

            // Nếu đúng, chuyển đến trang đổi mật khẩu
            TempData["UserIdForReset"] = user.Id;
            return RedirectToAction("DoiMatKhauMoi");
        }

        [HttpGet]
        public IActionResult DoiMatKhauMoi()
        {
            if (TempData["UserIdForReset"] == null)
                return RedirectToAction("QuenMatKhau");
            ViewBag.UserId = TempData["UserIdForReset"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhauMoi(string userId, string matKhauMoi)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("QuenMatKhau");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, matKhauMoi);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Hãy đăng nhập lại.";
                return RedirectToAction("DangNhap");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null) { return RedirectToAction("TrangChu", "Home"); }

            var danhSachDiaChi = _unit.DiaChiNhanHangs
                .GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang)
                .OrderByDescending(dc => dc.LaMacDinh)
                .ToList();
            var donHang = _unit.DonHangs.GetRange(dh => dh.MaKhachHang == khachHang.MaKhachHang);
            int soLuongDonHang = donHang.Count();

            var model = new ThongTinCaNhanVM
            {
                ThongTinKhachHang = khachHang,
                SoLuongDonHang = soLuongDonHang
            };

            // ⭐ BẮT ĐẦU THÊM MỚI: Tải dữ liệu cho Dropdown
            model.CacNgay = Enumerable.Range(1, 31)
                .Select(n => new SelectListItem { Value = n.ToString(), Text = n.ToString() }).ToList();

            model.CacThang = Enumerable.Range(1, 12)
                .Select(n => new SelectListItem { Value = n.ToString(), Text = n.ToString() }).ToList();

            // Lấy 100 năm (từ năm nay lùi 100 năm)
            model.CacNam = Enumerable.Range(DateTime.Now.Year - 100, 101)
                .Select(n => new SelectListItem { Value = n.ToString(), Text = n.ToString() })
                .OrderByDescending(n => n.Value).ToList();

            // Gán giá trị ngày sinh hiện tại (nếu có)
            if (khachHang.NgaySinh.HasValue)
            {
                model.NgaySinh = khachHang.NgaySinh.Value.Day;
                model.ThangSinh = khachHang.NgaySinh.Value.Month;
                model.NamSinh = khachHang.NgaySinh.Value.Year;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiMaXacThucCapNhat(string emailMoi)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return Unauthorized(); }

            if (string.IsNullOrEmpty(emailMoi))
            {
                return Json(new { success = false, message = "Vui lòng nhập email mới." });
            }

            // Gửi OTP đến email CŨ (user.Email)
            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("UpdateProfileOTP", otp);
            HttpContext.Session.SetString("UpdateProfileOTPExpiry", DateTime.UtcNow.AddMinutes(5).ToString("o"));

            try
            {
                await _emailSender.SendEmailAsync(
                    emailMoi, // <-- GỬI ĐẾN EMAIL CŨ (AN TOÀN)
                    "Mã Xác Thực Thay Đổi Thông Tin",
                    $"Mã xác thực của bạn là: <strong>{otp}</strong>. Mã này sẽ hết hạn sau 5 phút.");

                return Json(new { success = true, message = $"Đã gửi mã đến {emailMoi}." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi email." });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanCapNhat(ThongTinCaNhanVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == user.Id);
            if (user == null || khachHang == null) { return Unauthorized(); }

            // 1. Kiểm tra OTP
            var sessionOtp = HttpContext.Session.GetString("UpdateProfileOTP");
            var expiryTimeStr = HttpContext.Session.GetString("UpdateProfileOTPExpiry");

            if (string.IsNullOrEmpty(sessionOtp) || sessionOtp != model.MaOTP)
            {
                return Json(new { success = false, message = "Mã OTP không chính xác." });
            }
            if (DateTime.TryParse(expiryTimeStr, out var expiryTime) && expiryTime < DateTime.UtcNow)
            {
                return Json(new { success = false, message = "Mã OTP đã hết hạn." });
            }

            // 2. OTP Hợp Lệ -> Xóa OTP
            HttpContext.Session.Remove("UpdateProfileOTP");
            HttpContext.Session.Remove("UpdateProfileOTPExpiry");

            bool emailChanged = false;
            bool phoneChanged = false;

            // Lấy email/phone MỚI từ form
            string newEmail = model.ThongTinKhachHang.Email;
            string newPhone = model.ThongTinKhachHang.DienThoai;

            // 3. Cập nhật Email (nếu có thay đổi)
            if (!string.IsNullOrEmpty(newEmail) && user.Email != newEmail)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, newEmail);
                if (!setEmailResult.Succeeded) { /* Xử lý lỗi */ }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, newEmail);
                user.EmailConfirmed = true;
                khachHang.Email = newEmail;
                emailChanged = true;
            }

            // 4. Cập nhật SĐT (nếu có thay đổi)
            if (!string.IsNullOrEmpty(newPhone) && user.PhoneNumber != newPhone)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newPhone);
                if (!setPhoneResult.Succeeded) { /* Xử lý lỗi */ }
                user.PhoneNumberConfirmed = true;
                khachHang.DienThoai = newPhone;
                phoneChanged = true;
            }

            // 5. Lưu thay đổi
            await _userManager.UpdateAsync(user);
            _unit.KhachHangs.Update(khachHang);
            await _unit.SaveAsync();

            if (emailChanged) { await _signInManager.RefreshSignInAsync(user); }

            return Json(new
            {
                success = true,
                message = "Cập nhật thông tin thành công!",
                newEmail = khachHang.Email,
                newPhone = khachHang.DienThoai
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauThongTinCaNhanVM model)
        {
            // 1. Kiểm tra validation cơ bản (Required, Compare)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Json(new { success = false, message = string.Join(" ", errors) });
            }

            // 2. Lấy người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // 3. Thực hiện thay đổi (Identity tự kiểm tra mật khẩu cũ)
            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                model.MatKhauHienTai,
                model.MatKhauMoi
            );

            if (changePasswordResult.Succeeded)
            {
                return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
            }

            // 4. Trả về lỗi nếu thất bại (sai mật khẩu cũ, mật khẩu mới không đủ mạnh...)
            var identityErrors = changePasswordResult.Errors.Select(e => e.Description);
            return Json(new { success = false, message = string.Join(" ", identityErrors) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSo(ThongTinCaNhanVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null) { return Unauthorized(); }

            // 1. Cập nhật Database
            // (Lưu ý: model.ThongTinKhachHang.HoTen sẽ được bind từ form)
            khachHang.HoTen = model.ThongTinKhachHang.HoTen;

            // (Xử lý Giới tính - Giả sử bạn thêm thuộc tính GioiTinh vào VM)
            khachHang.GioiTinh = model.gender;

            // Ghép ngày sinh từ dropdown
            try
            {
                khachHang.NgaySinh = new DateTime(model.NamSinh, model.ThangSinh, model.NgaySinh);
            }
            catch
            {
                // Ngày không hợp lệ (ví dụ: 30/02), có thể bỏ qua hoặc báo lỗi
                // Bỏ qua:
                khachHang.NgaySinh = null;
            }

            _unit.KhachHangs.Update(khachHang);
            await _unit.SaveAsync();

            // 2. Cập nhật Cookie (Claim) để Header hiển thị tên mới
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var oldHoTenClaim = User.FindFirst("HoTen");
                if (oldHoTenClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, oldHoTenClaim);
                }
                await _userManager.AddClaimAsync(user, new Claim("HoTen", khachHang.HoTen));

                // Refresh cookie
                await _signInManager.RefreshSignInAsync(user);
            }

            return Json(new
            {
                success = true,
                message = "Cập nhật hồ sơ thành công!",
                hoTenMoi = khachHang.HoTen, // Trả về tên mới để JS cập nhật
                gioiTinh = (int?)khachHang.GioiTinh,
                ngaySinh = khachHang.NgaySinh?.Day,
                thangSinh = khachHang.NgaySinh?.Month,
                namSinh = khachHang.NgaySinh?.Year
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DanhSachDonHangPartial()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            if (khachHang == null)
            {
                return Unauthorized();
            }

            // Lấy tất cả đơn hàng, bao gồm Vận Chuyển (để lấy trạng thái)
            // và Chi Tiết (để lấy sản phẩm)
            var danhSachDonHang = await _unit.DonHangs.GetRangeAsync(
                filter: dh => dh.MaKhachHang == khachHang.MaKhachHang,
                orderBy: q => q.OrderByDescending(dh => dh.NgayTao), // Mới nhất lên đầu
                includeProperties: "VanChuyen,ChiTietDonHangs.Sach" // Quan trọng
            );

            // ⭐ NÂNG CẤP LOGIC PROJECT SANG VIEWMODEL
            var viewModelList = danhSachDonHang.Select(dh => {

                // Lấy chi tiết sản phẩm đầu tiên
                var sanPhamDauTien = dh.ChiTietDonHangs.FirstOrDefault();

                // Đếm số loại sản phẩm khác còn lại
                int soLuongSanPhamKhac = dh.ChiTietDonHangs.Count - 1; // (-1 vì đã lấy cái đầu tiên)

                return new DanhSachDonHangVM
                {
                    MaDonHang = dh.MaDonHang,
                    TrangThai = dh.VanChuyen?.TrangThaiGiaoHang ?? TrangThaiGiaoHang.ChoXuLy,
                    TongTien = dh.Total,
                    NgayDat = dh.NgayTao,

                    // Dùng '?' để tránh lỗi NullReferenceException nếu đơn hàng rỗng (bị lỗi)
                    TenSanPhamDauTien = sanPhamDauTien?.Sach?.TenSach ?? "Lỗi đơn hàng",
                    HinhAnhDauTien = sanPhamDauTien?.Sach?.AnhBiaChinh ?? "default-image.png",
                    SoLuongSanPhamDauTien = sanPhamDauTien?.SoLuong ?? 0,

                    // Đảm bảo số không bị âm nếu đơn hàng rỗng
                    SoLuongSanPhamKhac = soLuongSanPhamKhac < 0 ? 0 : soLuongSanPhamKhac
                };
            }).ToList();

            return PartialView("_DanhSachDonHangPartial", viewModelList);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> HuyDonHang(int maDonHang)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            var donHang = await _unit.DonHangs.GetAsync(
                dh => dh.MaDonHang == maDonHang && dh.MaKhachHang == khachHang.MaKhachHang,
                includeProperties: "VanChuyen,HoaDon,ChiTietDonHangs.Sach"
            );

            if (donHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Chỉ cho phép hủy khi đang "Chờ xử lý"
            if (donHang.VanChuyen.TrangThaiGiaoHang != TrangThaiGiaoHang.ChoXuLy)
            {
                return Json(new { success = false, message = "Không thể hủy đơn hàng đang được vận chuyển." });
            }

            if (donHang.HoaDon == null)
            {
                return Json(new { success = false, message = "Lỗi: Không tìm thấy hóa đơn của đơn hàng." });
            }

            if (donHang.VanChuyen == null)
            {
                return Json(new { success = false, message = "Lỗi: Không tìm thấy thông tin vận chuyển của đơn hàng." });
            }

            // 4. Cập nhật trạng thái Vận Chuyển
            donHang.VanChuyen.TrangThaiGiaoHang = TrangThaiGiaoHang.DaHuy; // (Giả sử bạn có enum này)
            _unit.VanChuyens.Update(donHang.VanChuyen); // <-- ⭐ BẮT BUỘC PHẢI GỌI UPDATE

            // 5. Cập nhật trạng thái Hóa Đơn
            donHang.HoaDon.TrangThai = TrangThaiHoaDon.DaHuy;
            donHang.HoaDon.TongTien = 0; // Ghi đè về 0
            donHang.HoaDon.VAT = 0;      // Ghi đè về 0
            _unit.HoaDons.Update(donHang.HoaDon); // <-- ⭐ BẮT BUỘC PHẢI GỌI UPDATE

            // ⭐ Hoàn lại tồn kho
            foreach (var item in donHang.ChiTietDonHangs)
            {
                var sanPham = _unit.Saches.GetById(item.MaSach);
                if (sanPham != null)
                {
                    sanPham.SoLuong += item.SoLuong;
                    _unit.Saches.Update(sanPham);
                }
            }

            await _unit.SaveAsync();

            return Json(new { success = true, message = "Đã hủy đơn hàng thành công." });
        }

        // ⭐ MỚI: Action [HttpGet] để tải riêng danh sách địa chỉ
        [HttpGet]
        public IActionResult DanhSachDiaChi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            var danhSachDiaChi = _unit.DiaChiNhanHangs
                .GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaDaXoa == false)
                .OrderByDescending(dc => dc.LaMacDinh)
                .ToList();

            return PartialView("_DanhSachDiaChiPartial", danhSachDiaChi);
        }


        // [HttpPost] LuuDiaChi (SỬA LẠI: Trả về PartialView khi lỗi, Json khi thành công)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiaChi(DiaChiNhanHang diaChi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null) { return Unauthorized(); }

            diaChi.MaKhachHang = khachHang.MaKhachHang;

            if (diaChi.LaMacDinh)
            {
                var cacDiaChiKhac = _unit.DiaChiNhanHangs
                    .GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.MaDiaChi != diaChi.MaDiaChi
                    && dc.LaDaXoa == false)
                    .ToList();
                foreach (var dc in cacDiaChiKhac)
                {
                    dc.LaMacDinh = false;
                    _unit.DiaChiNhanHangs.Update(dc);
                }
            }
            else
            {
                var soLuongDiaChi = _unit.DiaChiNhanHangs.Count(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaDaXoa == false);
                if (soLuongDiaChi == 0 && diaChi.MaDiaChi == 0) // Thêm mới địa chỉ đầu tiên
                {
                    diaChi.LaMacDinh = true;
                }
                else if (soLuongDiaChi > 0 && diaChi.MaDiaChi == 0) // Thêm mới khi đã có địa chỉ
                {
                    // (Không set mặc định, giữ nguyên diaChi.LaMacDinh = false)
                }
                else if (diaChi.MaDiaChi != 0) // Đang sửa
                {
                    // Kiểm tra xem đây có phải địa chỉ DUY NHẤT không
                    if (soLuongDiaChi == 1)
                    {
                        diaChi.LaMacDinh = true; // Nếu là cái duy nhất, ép nó làm mặc định
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // ⭐ SỬA: Trả về PartialView với lỗi validation
                // JavaScript sẽ nhận HTML này và cập nhật lại popup
                return PartialView("_ThemSuaDiaChiPartial", diaChi);
            }

            if (diaChi.MaDiaChi == 0)
            {
                _unit.DiaChiNhanHangs.Add(diaChi);
            }
            else
            {
                _unit.DiaChiNhanHangs.Update(diaChi);
            }

            await _unit.SaveAsync();

            // ⭐ SỬA: Trả về JSON thành công
            return Json(new { success = true, message = "Đã lưu địa chỉ thành công!", maDiaChiMoi = diaChi.MaDiaChi, maTinhThanhMoi = diaChi.MaTinhThanh, laMacDinh = diaChi.LaMacDinh });
        }

        // [HttpGet] ThemDiaChiPartial (GIỮ NGUYÊN)
        [HttpGet]
        public IActionResult ThemDiaChiPartial()
        {
            var model = new DiaChiNhanHang();
            return PartialView("_ThemSuaDiaChiPartial", model);
        }

        // [HttpGet] SuaDiaChiPartial (GIỮ NGUYÊN)
        [HttpGet]
        public async Task<IActionResult> SuaDiaChiPartial(int maDiaChi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null) { return Unauthorized(); }

            var diaChi = await _unit.DiaChiNhanHangs.GetByIdAsync(maDiaChi);
            if (diaChi == null || diaChi.MaKhachHang != khachHang.MaKhachHang)
            {
                return NotFound("Không tìm thấy địa chỉ.");
            }

            return PartialView("_ThemSuaDiaChiPartial", diaChi);
        }

        // [HttpPost] XoaDiaChi (SỬA LẠI: Bỏ TempData, chỉ trả về Json)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDiaChi(int maDiaChi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            var donHang = _unit.DonHangs.Get(dh => dh.MaKhachHang == khachHang.MaKhachHang);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Lỗi xác thực." });
            }

            var diaChi = await _unit.DiaChiNhanHangs.GetAsync(dc => dc.MaDiaChi == maDiaChi && dc.MaKhachHang == khachHang.MaKhachHang);
            if (diaChi == null)
            {
                return Json(new { success = false, message = "Không tìm thấy địa chỉ." });
            }

            if (diaChi.LaMacDinh)
            {
                // Lấy tất cả các địa chỉ CÒN LẠI của khách hàng
                var cacDiaChiKhac = await _unit.DiaChiNhanHangs.GetRangeAsync(
                    dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.MaDiaChi != maDiaChi && dc.LaDaXoa == false,
                    orderBy: q => q.OrderBy(dc => dc.MaDiaChi) // Sắp xếp để lấy cái "tiếp theo"
                );

                // Nếu còn địa chỉ khác (không phải là cái cuối cùng)
                if (cacDiaChiKhac.Any())
                {
                    // Lấy địa chỉ đầu tiên trong danh sách còn lại
                    var diaChiMoiMacDinh = cacDiaChiKhac.First();

                    // Gán nó làm mặc định
                    diaChiMoiMacDinh.LaMacDinh = true;
                    _unit.DiaChiNhanHangs.Update(diaChiMoiMacDinh);
                }
                // else: Nếu đây là địa chỉ cuối cùng, không cần làm gì.
                // Nó sẽ bị xóa ở BƯỚC 4 và khách hàng sẽ không còn địa chỉ nào.
            }

            diaChi.LaDaXoa = true;
            diaChi.LaMacDinh = false;
            _unit.DiaChiNhanHangs.Update(diaChi);
            await _unit.SaveAsync();

            return Json(new { success = true, message = "Xóa địa chỉ thành công." });
        }

        // [HttpPost] DatMacDinh (SỬA LẠI: Bỏ TempData, chỉ trả về Json)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatMacDinh(int maDiaChi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Lỗi xác thực." });
            }

            var diaChiMoiMacDinh = await _unit.DiaChiNhanHangs.GetAsync(
                dc => dc.MaDiaChi == maDiaChi &&
                    dc.MaKhachHang == khachHang.MaKhachHang &&
                    dc.LaDaXoa == false
            );
            if (diaChiMoiMacDinh == null || diaChiMoiMacDinh.MaKhachHang != khachHang.MaKhachHang)
            {
                return Json(new { success = false, message = "Không tìm thấy địa chỉ." });
            }

            var diaChiCuMacDinh = _unit.DiaChiNhanHangs.Get(
                dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaMacDinh
            );

            if (diaChiCuMacDinh != null)
            {
                diaChiCuMacDinh.LaMacDinh = false;
                _unit.DiaChiNhanHangs.Update(diaChiCuMacDinh);
            }

            diaChiMoiMacDinh.LaMacDinh = true;
            _unit.DiaChiNhanHangs.Update(diaChiMoiMacDinh);

            await _unit.SaveAsync();

            return Json(new { success = true, message = "Đã cập nhật địa chỉ mặc định." });
        }
    }
}