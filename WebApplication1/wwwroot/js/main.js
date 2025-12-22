const heartButtons = document.querySelectorAll(".deal-icon, .dp-icon");
heartButtons.forEach(icon => {
    icon.addEventListener("click", function() {
        this.classList.toggle("active");

        if(this.classList.contains("active")) {
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