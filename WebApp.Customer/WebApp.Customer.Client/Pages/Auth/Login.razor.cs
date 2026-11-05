using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices.JavaScript;
using WebApp.Shared.Dtos.Customer.Auth;
using JSException = Microsoft.JSInterop.JSException;

namespace WebApp.Customer.Client.Pages.Auth
{
    public partial class Login
    {
        private CustomerLoginDto loginModel = new("", "");
        private bool rememberMe = false;
        private bool isSubmitting = false;
        private string? errorMessage;
        // Các thông báo lỗi trả về từ API
        private List<string>? validationErrors;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            {
                try
                {
                    var saveEmail = await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "savedEmail");
                    if (!string.IsNullOrWhiteSpace(saveEmail))
                    {
                        loginModel = loginModel with { EmailOrPhone = saveEmail };
                        rememberMe = true;
                        StateHasChanged();
                    }
                }
                catch (JSException)
                {
                    // Người dùng có thể bật chặn Cookie trong trình duyệt hoặc chế độ ẩn danh
                }
            }
        }

        private async Task HandleSubmitAsync()
        {
            isSubmitting = true;
            errorMessage = null;
            validationErrors = null;

            try
            {
                try
                {
                    if (rememberMe)
                    {
                        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "savedEmail", loginModel.EmailOrPhone);
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "savedEmail");
                    }
                }
                catch (JSException){ }

                var result = await AuthService.LoginAsync(loginModel);

                if (result.Success && result.Data != null)
                {
                    // Nếu đăng nhập thành công thì force load tải lại trang để các request lần sau có đính kèm Cookie
                    Navigation.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    errorMessage = result.Message ?? "Tên đăng nhập hoặc mật khẩu không chính xác.";
                    validationErrors = result.Errors;
                }
            }
            catch (Exception)
            {
                errorMessage = "Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng.";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
