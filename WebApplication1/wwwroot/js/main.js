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

document.addEventListener('DOMContentLoaded', () => {
    // 1. Lấy các phần tử dựa trên class HTML cũ của bạn
    const sliderTrack = document.querySelector('.slider-item'); // Khung chứa các ảnh
    const dots = document.querySelectorAll('.dots i'); // Các dấu chấm tròn
    const container = document.getElementById('slider-container'); // Khung bao ngoài cùng

    // Kiểm tra an toàn: Nếu không tìm thấy slider thì dừng code để tránh lỗi
    if (!sliderTrack || !container) {
        console.warn("Không tìm thấy slider trong HTML. Kiểm tra lại class '.slider-item' hoặc id 'slider-container'.");
        return;
    }

    // Đếm số lượng ảnh
    const slides = sliderTrack.children;
    const totalSlides = slides.length;

    // Nếu không có ảnh nào hoặc số dot không khớp số ảnh, code vẫn chạy nhưng cần lưu ý
    if (totalSlides === 0) return;

    let currentIndex = 0;
    let autoPlayInterval;
    const timeDelay = 3500; // Thời gian chuyển slide (3.5 giây)

    // 2. Hàm chuyển Slide
    function goToSlide(index) {
        // Xử lý vòng lặp: Nếu quá cuối thì về 0, nếu nhỏ hơn 0 thì về cuối
        if (index >= totalSlides) index = 0;
        if (index < 0) index = totalSlides - 1;

        currentIndex = index;

        // Di chuyển khung ảnh bằng CSS transform
        // Lưu ý: CSS của bạn phải có .slider-item { display: flex; transition: transform... }
        sliderTrack.style.transform = `translateX(-${currentIndex * 100}%)`;

        // Cập nhật trạng thái Dot (Dựa trên FontAwesome class cũ của bạn)
        updateDots();
    }

    // 3. Hàm cập nhật giao diện chấm tròn
    function updateDots() {
        dots.forEach((dot, idx) => {
            if (idx === currentIndex) {
                // Dot đang chọn: Dùng hình tròn đặc (fa-solid)
                dot.classList.remove('fa-regular');
                dot.classList.add('fa-solid');
                dot.style.opacity = '1'; // Làm đậm lên
            } else {
                // Dot chưa chọn: Dùng viền tròn (fa-regular)
                dot.classList.remove('fa-solid');
                dot.classList.add('fa-regular');
                dot.style.opacity = '0.5'; // Làm mờ đi
            }
        });
    }

    // 4. Xử lý sự kiện Click vào Dot
    dots.forEach((dot, index) => {
        dot.style.cursor = 'pointer'; // Thêm con trỏ tay để biết click được
        dot.addEventListener('click', () => {
            stopAutoPlay(); // Dừng tự chạy khi người dùng tương tác
            goToSlide(index); // Chuyển đến slide tương ứng
            startAutoPlay(); // Chạy lại sau khi click
        });
    });

    // 5. Tự động chạy (Auto Play)
    function startAutoPlay() {
        // Xóa interval cũ nếu có để tránh chạy chồng chéo
        stopAutoPlay();
        autoPlayInterval = setInterval(() => {
            goToSlide(currentIndex + 1);
        }, timeDelay);
    }

    function stopAutoPlay() {
        if (autoPlayInterval) {
            clearInterval(autoPlayInterval);
        }
    }

    // 6. Tạm dừng khi rê chuột vào Slider
    container.addEventListener('mouseenter', stopAutoPlay);
    container.addEventListener('mouseleave', startAutoPlay);

    // --- KHỞI CHẠY LẦN ĐẦU ---
    updateDots(); // Cập nhật dot đầu tiên
    startAutoPlay(); // Bắt đầu chạy
});