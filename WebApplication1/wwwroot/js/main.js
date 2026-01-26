$(document).ready(function () {
    // --- XỬ LÝ WISHLIST (CÁCH MỚI: EVENT DELEGATION) ---
    // Bắt sự kiện click vào bất kỳ phần tử nào có class '.btn-add-wishlist'
    $(document).on('click', '.btn-add-wishlist', function (e) {
        e.preventDefault();      // Ngăn chặn hành vi mặc định (nếu có)
        e.stopPropagation();     // Ngăn chặn sự kiện nổi bọt lên thẻ cha (thẻ <a>)

        const icon = $(this);           // Lấy chính icon đang được click
        const productId = icon.data('id'); // Lấy ID sản phẩm từ data-id

        // Gửi yêu cầu AJAX về Server
        $.post('/Wishlist/Toggle', { id: productId }, function (response) {
            if (response.success) {
                if (response.liked) {
                    // Nếu server báo Đã thích -> Đổi sang icon đặc, màu đỏ
                    icon.removeClass('fa-regular').addClass('fa-solid').css('color', 'red');
                } else {
                    // Nếu server báo Bỏ thích -> Đổi sang icon rỗng, màu thường
                    icon.removeClass('fa-solid').addClass('fa-regular').css('color', '');
                }
            } else {
                alert("Có lỗi khi thao tác!");
            }
        }).fail(function () {
            alert("Lỗi kết nối tới server!");
        });
    });
});

// --- PHẦN 2: SLIDER (Chạy Banner trang chủ) ---
document.addEventListener('DOMContentLoaded', () => {
    // 1. Lấy các phần tử
    const sliderTrack = document.querySelector('.slider-item');
    const dots = document.querySelectorAll('.dots i');
    const container = document.getElementById('slider-container');

    // QUAN TRỌNG: Nếu không tìm thấy slider (ví dụ đang ở trang Wishlist),
    // thì chỉ cần return để dừng code lại, KHÔNG cần báo lỗi console.warn nữa.
    if (!sliderTrack || !container) {
        return;
    }

    // --- Code xử lý slider (giữ nguyên logic cũ của bạn) ---
    const slides = sliderTrack.children;
    const totalSlides = slides.length;
    if (totalSlides === 0) return;

    let currentIndex = 0;
    let autoPlayInterval;
    const timeDelay = 3500;

    function goToSlide(index) {
        if (index >= totalSlides) index = 0;
        if (index < 0) index = totalSlides - 1;
        currentIndex = index;
        sliderTrack.style.transform = `translateX(-${currentIndex * 100}%)`;
        updateDots();
    }

    function updateDots() {
        dots.forEach((dot, idx) => {
            if (idx === currentIndex) {
                dot.classList.remove('fa-regular');
                dot.classList.add('fa-solid');
                dot.style.opacity = '1';
            } else {
                dot.classList.remove('fa-solid');
                dot.classList.add('fa-regular');
                dot.style.opacity = '0.5';
            }
        });
    }

    dots.forEach((dot, index) => {
        dot.style.cursor = 'pointer';
        dot.addEventListener('click', () => {
            stopAutoPlay();
            goToSlide(index);
            startAutoPlay();
        });
    });

    function startAutoPlay() {
        stopAutoPlay();
        autoPlayInterval = setInterval(() => {
            goToSlide(currentIndex + 1);
        }, timeDelay);
    }

    function stopAutoPlay() {
        if (autoPlayInterval) clearInterval(autoPlayInterval);
    }

    container.addEventListener('mouseenter', stopAutoPlay);
    container.addEventListener('mouseleave', startAutoPlay);

    updateDots();
    startAutoPlay();
});