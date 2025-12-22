
const wishlistIcons = document.querySelectorAll('.wl-icon');

wishlistIcons.forEach(icon => {
    icon.addEventListener("click", () => {
        // Nếu icon chưa được "like"
        if (!icon.classList.contains("liked")) {
            icon.classList.add("liked");
            icon.style.color = "#ff4d4d";
        }
        else {

            icon.classList.remove("liked");
            icon.style.color = "#ccc";

            alert("Đã loại bỏ sản phẩm khỏi danh sách yêu thích");
        }
    });
});