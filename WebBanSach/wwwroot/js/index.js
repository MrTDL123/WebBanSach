document.addEventListener("DOMContentLoaded", function () {

    // ===================================
    // PHẦN 1: SLIDESHOW
    // ===================================
    const slides = document.querySelectorAll("#slide_container section");
    const buttonLeft = document.querySelector("#btn_slideleft");
    const buttonRight = document.querySelector("#btn_slideright");

    // Chỉ chạy code slideshow nếu tìm thấy slide
    if (slides.length > 0) {
        let currentIndex = 0;
        const length = slides.length;

        // Xử lí trình tự xuất hiện của các slide khi web đc load
        slides.forEach((slide, index) => {
            if (index === 0) {
                slide.classList.add("show");
            } else {
                slide.classList.remove("show");
            }
        });

        // NÚT CHỈNH SLIDES
        const handleChangeSlide = function () {
            slides[currentIndex].classList.remove("show");
            currentIndex = (currentIndex + 1) % length;
            slides[currentIndex].classList.add("show");
        }

        let handleEventChangeSlide = setInterval(handleChangeSlide, 6000);

        // Chỉ thêm sự kiện nếu nút tồn tại
        if (buttonRight) {
            buttonRight.addEventListener('click', function () {
                clearInterval(handleEventChangeSlide);
                handleChangeSlide();
                handleEventChangeSlide = setInterval(handleChangeSlide, 6000);
            });
        }

        // Chỉ thêm sự kiện nếu nút tồn tại
        if (buttonLeft) {
            buttonLeft.addEventListener('click', function () {
                clearInterval(handleEventChangeSlide);
                slides[currentIndex].classList.remove("show");
                currentIndex = (currentIndex === 0) ? length - 1 : currentIndex - 1;
                slides[currentIndex].classList.add("show");
                handleEventChangeSlide = setInterval(handleChangeSlide, 6000);
            });
        }
    }

    // ===================================
    // PHẦN 2: ĐỒNG HỒ ĐẾM NGƯỢC
    // ===================================

    // 1. THIẾT LẬP NGÀY KẾT THÚC CỦA SỰ KIỆN
    const eventDate = new Date("Jan 1, 2026 00:00:00").getTime();

    // 2. LẤY CÁC ELEMENT TỪ HTML
    const daysEl = document.getElementById('days');
    const hoursEl = document.getElementById('hours');
    const minutesEl = document.getElementById('minutes');
    const secondsEl = document.getElementById('seconds');
    const countdownEl = document.getElementById('countdown');
    const eventMessageEl = document.getElementById('event-message');

    // Hàm thêm số 0
    const formatTime = (time) => time < 10 ? `0${time}` : time;

    // 3. CẬP NHẬT ĐỒNG HỒ MỖI GIÂY
    // Chỉ chạy nếu các element tồn tại
    if (daysEl && hoursEl && minutesEl && secondsEl && countdownEl && eventMessageEl) {
        const countdownInterval = setInterval(() => {
            // Lấy thời gian hiện tại
            const now = new Date().getTime();

            // Tính khoảng cách thời gian còn lại
            const distance = eventDate - now;

            // Nếu thời gian đã hết
            if (distance < 0) {
                clearInterval(countdownInterval); // Dừng bộ đếm
                countdownEl.classList.add('hidden'); // Ẩn đồng hồ
                eventMessageEl.classList.remove('hidden'); // Hiện thông báo
                return;
            }

            // Tính toán ngày, giờ, phút, giây
            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            // Hiển thị kết quả lên giao diện
            daysEl.innerText = formatTime(days);
            hoursEl.innerText = formatTime(hours);
            minutesEl.innerText = formatTime(minutes);
            secondsEl.innerText = formatTime(seconds);

        }, 1000); // Cập nhật mỗi 1 giây (1000ms)
    }
});