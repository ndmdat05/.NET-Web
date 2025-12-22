function addToCart() {
    alert("Thêm vào giỏ hàng thành công");
}

//Tăng giảm value khi nhấn vào button
const minusBtn = document.querySelector(".minus");
const plusBtn = document.querySelector(".plus");
const qtyInput = document.querySelector(".quantity-control input");
plusBtn.onclick = () => {
    qtyInput.value = Number(qtyInput.value) + 1;
};

minusBtn.onclick = () => {
    if (qtyInput.value > 1) {
        qtyInput.value = Number(qtyInput.value) - 1;
    }
};

//Thay đổi trạng thái button size
const sizeButtons = document.querySelectorAll(".size-options button");
sizeButtons.forEach(btn => {
    btn.onclick = () => {
        //Khi click vào nếu disable thì sẽ không có gì xảy ra
        if (btn.classList.contains("disabled")) return;
        //
        sizeButtons.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");
    };
});
const heartButton = document.querySelectorAll(".fa-heart");
heartButton.forEach(icon => {
    icon.addEventListener("click", function() {
        this.classList.toggle("active");

        if(this.classList.contains("active")) {
            alert("Đã thêm sản phẩm vào yêu thích thành công!");
            this.classList.remove("fa-regular");
            this.classList.add("fa-solid");
            this.style.color = "red";
        }
        else{
            alert("Đã xoá sản phẩm khỏi danh sách yêu thích")
            this.classList.remove("fa-solid");
            this.classList.add("fa-regular");
            this.style.color = "";
        }
    })
})