using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text.Json;
using WebApp.Shared.Dtos.Common;

namespace WebApp.Api.Middlewares
{
    // Custom Middleware thực hiện FluentValidation trước khi đưa dữ liệu về Controller 
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra Endpoint có trỏ đến một Controller Action nào không
            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (actionDescriptor == null)
            {
                await _next(context);
                return;
            }

            // Tìm DTO truyền dựa vào Body request
            var dtoParameter = actionDescriptor.Parameters
                .FirstOrDefault(p => p.BindingInfo?.BindingSource?.Id == "Body"
                                || (!p.ParameterType.IsPrimitive && p.ParameterType != typeof(string)));

            if (dtoParameter == null || context.Request.ContentLength == 0)
            {
                await _next(context);
                return;
            }

            var dtoType = dtoParameter.ParameterType;

            // Tìm Validator tương ứng với DTO trong DI Container
            var validatorType = typeof(IValidator<>).MakeGenericType(dtoType);
            var validator = context.RequestServices.GetService(validatorType) as IValidator;

            if (validator == null) 
            {
                await _next(context);
                return;
            }

            // Cho phép đọc Request Body
            context.Request.EnableBuffering();

            object? dtoInstance = null;
            try
            {
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var bodyText = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    dtoInstance = JsonSerializer.Deserialize(bodyText, dtoType, _jsonOptions);
                }
            }
            catch (JsonException)
            {
                await ReturnBadRequestResponseAsync(context, new List<string> { "Dữ liệu JSON gửi lên không đúng định dạng." });
                return;
            }
            finally
            {
                // Reset vị trí con trỏ về 0 để Controller đọc lại Body từ đầu
                context.Request.Body.Position = 0;
            }

            if (dtoInstance == null)
            {
                await ReturnBadRequestResponseAsync(context, new List<string> { "Dữ liệu Request Body không được để rỗng." });
                return;
            }


            // Thực hiện FluentValidation
            var validationContext = new ValidationContext<object>(dtoInstance);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                await ReturnBadRequestResponseAsync(context, errorMessages);
                return;
            }

            await _next(context);
        }

        private static async Task ReturnBadRequestResponseAsync(HttpContext context, List<string> errors)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var responseModel = new ApiResponse<object>(
                Success: false,
                Message: "Dữ liệu đầu vào không hợp lệ.",
                Data: null,
                Errors: errors
            );

            var jsonResponse = JsonSerializer.Serialize(responseModel, _jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
