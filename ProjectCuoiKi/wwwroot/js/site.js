document.addEventListener("DOMContentLoaded", function () {
    const slides = document.querySelectorAll("#slide_container section");
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
    const buttonLeft = document.querySelector("#btn_slideleft");
    const buttonRight = document.querySelector("#btn_slideright");

    const handleChangeSlide = function () {
        slides[currentIndex].classList.remove("show");
        currentIndex = (currentIndex + 1) % length;
        slides[currentIndex].classList.add("show");
    }

    let handleEventChangeSlide = setInterval(handleChangeSlide, 6000);

    buttonRight.addEventListener('click', function () {
        clearInterval(handleEventChangeSlide);
        handleChangeSlide();
        handleEventChangeSlide = setInterval(handleChangeSlide, 6000);
    });

    buttonLeft.addEventListener('click', function () {
        clearInterval(handleEventChangeSlide);
        slides[currentIndex].classList.remove("show");
        currentIndex = (currentIndex === 0) ? length - 1 : currentIndex - 1;
        slides[currentIndex].classList.add("show");
        handleEventChangeSlide = setInterval(handleChangeSlide, 6000);
    });
});
