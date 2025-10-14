
function Delete(url) {
    Swal.fire({
        title: "Bạn có chắc chắn muốn xóa không?",
        text: "Bạn sẽ không thể hoàn tác lại được!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Đồng ý xóa!"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}