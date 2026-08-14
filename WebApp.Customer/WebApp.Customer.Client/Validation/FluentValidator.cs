using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Customer.Client.Validation
{
    // Component kết nối AbsatrctValidator<T> tương ứng với DTO 
    public class FluentValidator : ComponentBase, IDisposable
    {
        private EditContext? _previousEditContext;
        private ValidationMessageStore? _messageStore;

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = default!;

        [CascadingParameter]
        // Đối tượng quản lý toàn bộ trạng thái dữ liệu, sự kiện của <EditForm>
        private EditContext? CurrentEditContext { get; set; }

        protected override void OnParametersSet()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FluentValidator)} bắt buộc phải nằm bên trong một <EditForm>.");
            }

            if (CurrentEditContext != _previousEditContext)
            {
                DetachEvents();

                _previousEditContext = CurrentEditContext;
                // Kho chứa thông báo lỗi 
                _messageStore = new ValidationMessageStore(CurrentEditContext);

                CurrentEditContext.OnValidationRequested += HandleValidationRequested;
                CurrentEditContext.OnFieldChanged += HandleFieldChanged;
            }
        }

        /// <summary>
        /// Xử lý khi người dùng gõ xong 1 ô input và rời con trỏ chuột
        /// </summary>
        private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            Console.WriteLine($"[FluentValidator Log] Field Changed: {e.FieldIdentifier.FieldName}");
            // Xóa thông báo cũ của ô nhập liệu đang xét
            _messageStore?.Clear(e.FieldIdentifier);
            ValidateField(CurrentEditContext!, e.FieldIdentifier);
        }

        /// <summary>
        /// Xử lý khi người dùng bấm Submit Form (Validate toàn bộ Model)
        /// </summary>
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            Console.WriteLine("[FluentValidator Log] Validation Requested (Form Submitted)");
            _messageStore?.Clear();
            ValidateModel(CurrentEditContext!);
        }

        private void ValidateModel(EditContext editContext)
        {
            var validator = GetValidatorForModel(editContext.Model);
            if (validator == null) return;

            var context = new ValidationContext<object>(editContext.Model);
            var failureResults = validator.Validate(context);

            foreach (var error in failureResults.Errors)
            {
                var fieldIdentifier = new FieldIdentifier(editContext.Model, error.PropertyName);
                _messageStore?.Add(fieldIdentifier, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private void ValidateField(EditContext editContext, FieldIdentifier fieldIdentifier)
        {
            var validator = GetValidatorForModel(fieldIdentifier.Model);
            if (validator == null) return;

            // Lọc các rule của riêng field này
            var context = ValidationContext<object>.CreateWithOptions(
                fieldIdentifier.Model,
                options => options.IncludeProperties(fieldIdentifier.FieldName));

            var failureResults = validator.Validate(context);

            foreach (var error in failureResults.Errors)
            {
                _messageStore?.Add(fieldIdentifier, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        // Lấy IValidator tương ứng trong DI container
        private IValidator? GetValidatorForModel(object model)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
            var validator = ServiceProvider.GetService(validatorType) as IValidator;

            if (validator == null)
            {
                Console.WriteLine($"[FluentValidator Warning] KHÔNG TÌM THẤY Validator nào được đăng ký cho Type: {model.GetType().Name}. Hãy kiểm tra lại Program.cs!");
            }

            return validator;
        }

        private void DetachEvents()
        {
            if (_previousEditContext != null)
            {
                _previousEditContext.OnValidationRequested -= HandleValidationRequested;
                _previousEditContext.OnFieldChanged -= HandleFieldChanged;
            }
        }

        public void Dispose()
        {
            DetachEvents();
        }
    }
}
