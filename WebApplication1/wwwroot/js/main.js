const heartButtons = document.querySelectorAll(".deal-icon, .dp-icon");
heartButtons.forEach(icon => {
    icon.addEventListener("click", function () {
        this.classList.toggle("active");

        if (this.classList.contains("active")) {
            alert("Đã thêm sản phẩm vào yêu thích thành công!");
            this.classList.remove("fa-regular");
            this.classList.add("fa-solid");
            this.style.color = "red";
        }
        else {
            alert("Đã xóa sản phẩm khỏi danh sách yêu thích");
            this.classList.remove("fa-solid");
            this.classList.add("fa-regular");
            this.style.color = "";
        }
    });
});

//Slider
document.addEventListener('DOMContentLoaded', () => {
    const sliderItem = document.querySelector('. slider-item');
    if (!sliderItem) return;

    // ĐẾM PHẦN TỬ CON TRỰC TIẾP thay vì đếm img
    const slides = Array.from(sliderItem.children);
    const dots = Array.from(document.querySelectorAll('.dots i'));
    const total = slides.length;
    let current = 0;
    const delay = 3500;
    let timer = null;

    function updateDots() {
        dots.forEach((dot, idx) => {
            if (idx < total) {
                dot.classList.toggle('fa-solid', idx === current);
                dot.classList.toggle('fa-regular', idx !== current);
            }
        });
    }

    function goTo(index) {
        if (total === 0) return;
        current = ((index % total) + total) % total;
        sliderItem.style.transform = `translateX(-${current * 100}%)`;
        updateDots();
    }

    function startAutoplay() {
        stopAutoplay();
        timer = setInterval(() => goTo(current + 1), delay);
    }

    function stopAutoplay() {
        if (timer) {
            clearInterval(timer);
            timer = null;
        }
    }

    // Click dot
    dots.forEach((dot, idx) => {
        dot.addEventListener('click', () => {
            goTo(idx);
            startAutoplay();
        });
    });

    // Pause on hover
    const container = document.getElementById('slider-container');
    if (container) {
        container.addEventListener('mouseenter', stopAutoplay);
        container.addEventListener('mouseleave', startAutoplay);
    }

    // Khởi tạo
    goTo(0);
    startAutoplay();
});