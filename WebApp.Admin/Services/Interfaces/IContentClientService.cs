using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IContentClientService
    {
        /// <summary>
        /// Lấy danh sách sản phẩm với phân trang và lọc.
        /// </summary> 
        Task<ApiResponse<PagedResultDto<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto);

        /// <summary>
        /// Quản lý sản phẩm: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sản phẩm.
        /// </summary>
        Task<ApiResponse<UpdateProductDto>> GetProductForUpdateAsync(int id);
        Task<ApiResponse<bool>> CreateProductAsync(CreateProductDto dto);
        Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductDto dto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);

        /// <summary>
        /// Lấy danh sách sách tủ sách 0 đồng với phân trang và lọc.
        /// </summary> 
        Task<ApiResponse<PagedResultDto<FreeBookListDto>>> GetFreeBooksAsync(FreeBookFilterDto filterDto);

        /// <summary>
        /// Quản lý sách tủ sách 0 đồng: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sách tủ sách 0 đồng.
        /// </summary>
        Task<ApiResponse<UpdateFreeBookDto>> GetFreeBookForUpdateAsync(int id);
        Task<ApiResponse<FreeBookDetailDto>> GetFreeBookDetailAsync(int id);
        Task<ApiResponse<bool>> CreateFreeBookAsync(CreateFreeBookDto dto);
        Task<ApiResponse<bool>> UpdateFreeBookAsync(UpdateFreeBookDto dto);
        Task<ApiResponse<bool>> DeleteFreeBookAsync(int id);

        /// <summary>
        /// Lấy danh sách các tác giả, nhà xuất bản, danh mục và voucher.
        /// </summary>
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();
        Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync();
        Task<ApiResponse<List<PublisherDto>>> GetPublishersAsync();
        Task<ApiResponse<List<VoucherDto>>> GetVouchersAsync();

        /// <summary>
        /// Quản lý danh mục, tác giả, nhà xuất bản và voucher: Lấy danh sách, tạo mới, cập nhật và xóa.
        /// </summary>
        Task<ApiResponse<bool>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ApiResponse<bool>> CreateAuthorAsync(CreateAuthorDto dto);
        Task<ApiResponse<bool>> CreatePublisherAsync(CreatePublisherDto dto);
        Task<ApiResponse<bool>> CreateVoucherAsync(CreateVoucherDto dto);
        Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task<ApiResponse<bool>> UpdateAuthorAsync(UpdateAuthorDto dto);
        Task<ApiResponse<bool>> UpdatePublisherAsync(UpdatePublisherDto dto);
        Task<ApiResponse<bool>> UpdateVoucherAsync(UpdateVoucherDto dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
        Task<ApiResponse<bool>> DeleteAuthorAsync(int id);
        Task<ApiResponse<bool>> DeletePublisherAsync(int id);
        Task<ApiResponse<bool>> DeleteVoucherAsync(int id);
    }
}
