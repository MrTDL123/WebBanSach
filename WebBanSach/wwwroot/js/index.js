/**
 * 1. HÀM XỬ LÝ SLIDESHOW
 */
function initSlideshow() {
    const slides = document.querySelectorAll("#slide_container section");
    const buttonLeft = document.querySelector("#btn_slideleft");
    const buttonRight = document.querySelector("#btn_slideright");

    if (slides.length === 0) return;

    let currentIndex = 0;
    const length = slides.length;

    // Set active slide đầu tiên
    slides.forEach((slide, index) => {
        slide.classList.toggle("show", index === 0);
    });

    const handleChangeSlide = function () {
        slides[currentIndex].classList.remove("show");
        currentIndex = (currentIndex + 1) % length;
        slides[currentIndex].classList.add("show");
    };

    let autoSlideInterval = setInterval(handleChangeSlide, 6000);

    const handleButtonClick = (callback) => {
        clearInterval(autoSlideInterval);
        callback();
        autoSlideInterval = setInterval(handleChangeSlide, 6000);
    };

    if (buttonRight) {
        buttonRight.addEventListener('click', () => {
            handleButtonClick(handleChangeSlide);
        });
    }

    if (buttonLeft) {
        buttonLeft.addEventListener('click', () => {
            handleButtonClick(() => {
                slides[currentIndex].classList.remove("show");
                currentIndex = (currentIndex === 0) ? length - 1 : currentIndex - 1;
                slides[currentIndex].classList.add("show");
            });
        });
    }
}

/**
 * 2. HÀM XỬ LÝ SCROLL FLASH SALE
 */
function initFlashSaleScroll() {
    const listContainer = document.getElementById('sach-giam-gia-section');
    const prevBtn = document.getElementById('btn_flashsale_prev');
    const nextBtn = document.getElementById('btn_flashsale_next');

    if (!listContainer || !prevBtn || !nextBtn) return;

    const getScrollAmount = () => {
        const bookItem = listContainer.querySelector('.book-item');
        if (!bookItem) return listContainer.clientWidth / 2;

        const itemWidth = bookItem.offsetWidth;
        const gap = 15; // Phải khớp với CSS gap
        return (itemWidth + gap) * 4; // Scroll 4 items
    };

    const updateButtons = () => {
        const maxScrollLeft = listContainer.scrollWidth - listContainer.clientWidth;

        // Nút Prev
        if (listContainer.scrollLeft <= 0) {
            prevBtn.style.opacity = '0';
            prevBtn.style.pointerEvents = 'none';
        } else {
            prevBtn.style.opacity = '0.8';
            prevBtn.style.pointerEvents = 'auto';
        }

        // Nút Next
        // Dùng sai số nhỏ (-1) để tránh lỗi làm tròn pixel trên một số màn hình
        if (listContainer.scrollLeft >= maxScrollLeft - 1) {
            nextBtn.style.opacity = '0';
            nextBtn.style.pointerEvents = 'none';
        } else {
            nextBtn.style.opacity = '0.8';
            nextBtn.style.pointerEvents = 'auto';
        }
    };

    listContainer.addEventListener('scroll', updateButtons);

    nextBtn.addEventListener('click', () => {
        listContainer.scrollBy({ left: getScrollAmount(), behavior: 'smooth' });
    });

    prevBtn.addEventListener('click', () => {
        listContainer.scrollBy({ left: -getScrollAmount(), behavior: 'smooth' });
    });

    // Init
    updateButtons();
    window.addEventListener('resize', updateButtons);
}

/**
 * 3. HÀM XỬ LÝ GIỎ HÀNG (JQUERY)
 * Đã nâng cấp dùng Event Delegation để bắt sự kiện tốt hơn
 */
function initAddToCart() {
    if (typeof $ === 'undefined') return;

    // Sử dụng $(document).on('click', selector, function)
    // Giúp code vẫn chạy kể cả khi nút .add-to-cart được sinh ra sau khi trang đã load
    $(document).on('click', '.add-to-cart', function (e) {
        e.preventDefault(); // Ngăn chặn hành vi mặc định nếu là thẻ a

        var btn = $(this);
        var maSach = btn.data('masach');

        // Hiệu ứng loading đơn giản (Optional)
        var originalText = btn.html();
        btn.prop('disabled', true).text('Đang thêm...');

        $.ajax({
            url: '/Customer/GioHang/ThemVaoGioHang',
            type: 'POST',
            data: { maSach: maSach, soLuong: 1 },
            success: function (res) {
                if (res.success) {
                    $('#cart-count').text(res.count);
                    // Có thể thêm thông báo Toast hoặc Alert ở đây nếu muốn
                    // alert("Thêm vào giỏ thành công!");
                }
            },
            error: function (err) {
                console.error("Lỗi thêm giỏ hàng:", err);
                alert("Có lỗi xảy ra, vui lòng thử lại.");
            },
            complete: function () {
                // Trả lại trạng thái nút sau khi chạy xong
                btn.prop('disabled', false).html(originalText);
            }
        });
    });
}

// ===================================
// MAIN EXECUTION
// ===================================
document.addEventListener("DOMContentLoaded", function () {
    // 1. Chạy Slide
    initSlideshow();

    // 2. Chạy Flash Sale Scroll
    initFlashSaleScroll();

    // 3. Chạy Giỏ hàng (Kiểm tra jQuery)
    if (typeof $ !== 'undefined') {
        $(document).ready(function () {
            initAddToCart();
        });
    } else {
        console.warn("jQuery chưa được load! Chức năng giỏ hàng sẽ không hoạt động.");
    }
});