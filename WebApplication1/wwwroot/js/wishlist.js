$(document).ready(function () {
    // 1. Bắt sự kiện click vào bất kỳ nút nào có class 'wishlist-btn'
    // (Dùng event delegation 'on' để bắt được cả những nút sinh ra sau khi load trang)
    $(document).on('click', '.wishlist-btn', function (e) {
        e.preventDefault(); // Chặn việc nhảy trang
        e.stopPropagation(); // Chặn sự kiện lan ra ngoài (quan trọng cho trang chủ)

        var btn = $(this);
        var icon = btn.find('i'); // Tìm icon trái tim bên trong nút

        // Nếu nút chính là icon (trường hợp trang chủ dùng thẻ <i> làm nút)
        if (btn.is('i')) {
            icon = btn;
        }

        var productId = btn.data('id'); // Lấy ID sản phẩm từ data-id="..."

        if (!productId) {
            console.error("Lỗi: Không tìm thấy ID sản phẩm!");
            return;
        }

        // 2. Gửi yêu cầu lên Server
        $.post('/Wishlist/Toggle', { id: productId }, function (response) {
            if (response.success) {
                if (response.liked) {
                    // Đã thích -> Đổi sang tim đỏ (Solid)
                    icon.removeClass('fa-regular').addClass('fa-solid').css('color', 'red');
                    // alert("Đã thêm vào yêu thích!"); 
                } else {
                    // Bỏ thích -> Đổi sang tim trắng (Regular)
                    icon.removeClass('fa-solid').addClass('fa-regular').css('color', '');
                    // alert("Đã xóa khỏi yêu thích!");
                }
            } else {
                // Nếu server trả về success: false
                if (response.message === "Vui lòng đăng nhập") {

                     if (confirm("Bạn cần đăng nhập để lưu yêu thích. Đi đến trang đăng nhập?")) {
                         window.location.href = "/Account/Login";
                     }
                    alert("Bạn chưa đăng nhập, nhưng sản phẩm vẫn được lưu tạm vào phiên làm việc!");
                } else {
                    alert("Có lỗi xảy ra: " + response.message);
                }
            }
        }).fail(function () {
            alert("Lỗi kết nối Server! Vui lòng thử lại sau.");
        });
    });
});