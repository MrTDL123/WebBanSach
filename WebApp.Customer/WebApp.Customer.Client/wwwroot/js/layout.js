// wwwroot/js/layout.js
document.addEventListener("DOMContentLoaded", () => {
    const header = document.querySelector('.co_dinh_header');
    const backtop = document.getElementById('backtop_container');
    const stickyThreshold = 200;
    const backtopThreshold = 300;

    // Sử dụng requestAnimationFrame để tối ưu hiệu năng sự kiện scroll thay vì bắt liên tục
    let isScrolling = false;

    window.addEventListener('scroll', () => {
        if (!isScrolling) {
            window.requestAnimationFrame(() => {
                const scrollPos = window.scrollY;

                // 1. Xử lý Header Sticky
                if (scrollPos >= stickyThreshold) {
                    header.classList.add('sticky');
                } else {
                    header.classList.remove('sticky');
                }

                // 2. Xử lý ẩn/hiện nút Backtop
                if (scrollPos > backtopThreshold) {
                    backtop.classList.add('show');
                } else {
                    backtop.classList.remove('show');
                }

                isScrolling = false;
            });
            isScrolling = true;
        }
    });
});

// Hàm gọi khi click nút back to top
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}