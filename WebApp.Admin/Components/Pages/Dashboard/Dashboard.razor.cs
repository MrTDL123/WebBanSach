using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebApp.Shared.Dtos.Management.Common;

namespace WebApp.Admin.Components.Pages.Dashboard
{
    public partial class Dashboard : IAsyncDisposable
    {
        private DashboardStatsDto? _dashBoardStateModel;
        private bool _isLoading = true;
        private IJSObjectReference? _jsModule;

        // Tab hiện tại đang chọn: 'revenue' | 'orders' | 'warehouse' | 'customers'
        private string _activeTab = "revenue";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // 2. Gọi qua Service Client
                var response = await DashboardClientService.GetDashboardStatsAsync();

                // Giả định response của bạn có bọc trạng thái Success/Data
                if (response.Success && response.Data != null)
                {
                    _dashBoardStateModel = response.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi nạp Dashboard: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_dashBoardStateModel != null && _jsModule == null)
            {
                try
                {
                    _jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./Components/Pages/Dashboard/Dashboard.razor.js");
                    await RenderCurrentTabChartAsync();
                }
                catch (JSException) { }  
            }
        }

        // Chuyển Tab khi người dùng bấm vào từng thẻ KPI
        private async Task SelectTabAsync(string tabName)
        {
            if (_activeTab == tabName) return;
            _activeTab = tabName;
            StateHasChanged();
            await Task.Delay(50); // Chờ DOM render canvas mới
            await RenderCurrentTabChartAsync();
        }

        private async Task RenderCurrentTabChartAsync()
        {
            if (_jsModule == null || _dashBoardStateModel == null) return;
            try
            {
                switch (_activeTab)
                {
                    case "revenue":
                        var labels = _dashBoardStateModel.MonthlyRevenues.Select(x => x.MonthLabel).ToArray();
                        var values = _dashBoardStateModel.MonthlyRevenues.Select(x => x.Revenue).ToArray();
                        await _jsModule.InvokeVoidAsync("renderRevenueChart", labels, values);
                        break;
                    case "orders":
                        await _jsModule.InvokeVoidAsync("renderOrderStatusChart",
                            _dashBoardStateModel.CompletedOrders,
                            _dashBoardStateModel.PendingOrders + _dashBoardStateModel.ProcessingOrders,
                            _dashBoardStateModel.ShippingOrders,
                            _dashBoardStateModel.CancelledOrders + _dashBoardStateModel.ReturnedOrders);
                        break;
                    case "warehouse":
                        var catLabels = _dashBoardStateModel.CategoryRevenues.Select(x => x.CategoryName).ToArray();
                        var catValues = _dashBoardStateModel.CategoryRevenues.Select(x => x.TotalRevenue).ToArray();
                        await _jsModule.InvokeVoidAsync("renderCategoryChart", catLabels, catValues);
                        break;
                    case "customers":
                        await _jsModule.InvokeVoidAsync("renderCustomerChart",
                            _dashBoardStateModel.CustomersWithOrders,
                            _dashBoardStateModel.CustomersWithoutOrders);
                        break;
                }
            }
            catch (JSException) { }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_jsModule is not null)
                {
                    await _jsModule.DisposeAsync();
                }
            }
            catch (JSDisconnectedException) { }
        }
    }
}
