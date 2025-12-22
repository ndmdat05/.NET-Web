
function extractNumber(text) {
    let result = "";
    for (let i = 0; i < text.length; i++) {
        const c = text[i];
        if (c >= '0' && c <= '9') result += c;
    }
    return Number(result);
}

function formatDot(value) {
    let str = value.toString();
    let result = "";
    let count = 0;

    for (let i = str.length - 1; i >= 0; i--) {
        result = str[i] + result;
        count++;

        if (count === 3 && i !== 0) {
            result = "." + result;
            count = 0;
        }
    }

    return result;
}

function formatMoney(value) {
    return formatDot(value) + "đ";
}

function updateCartTotal() {
    let sum = 0;

    document.querySelectorAll('.product-total').forEach(el => {
        sum += extractNumber(el.textContent) || 0;
    });

    document.querySelectorAll('.summary-value').forEach(el => {
        el.textContent = formatMoney(sum);
    });
}

document.querySelectorAll('.cart-item').forEach(item => {
    const priceEl = item.querySelector('.product-price');
    const qtyInput = item.querySelector('.qty-input');
    const totalEl = item.querySelector('.product-total');
    const plus = item.querySelector('.increase');
    const minus = item.querySelector('.decrease');
    const removeBtn = item.querySelector('.remove-item');
    const price = extractNumber(priceEl.textContent);

    function updateItemTotal() {
        const qty = Number(qtyInput.value);
        const total = qty * price;
        totalEl.textContent = formatMoney(total);
        updateCartTotal();
    }

    plus.addEventListener("click", () => {
        qtyInput.value = Number(qtyInput.value) + 1;
        updateItemTotal();
    });

    minus.addEventListener("click", () => {
        if (qtyInput.value > 1) {
            qtyInput.value = Number(qtyInput.value) - 1;
            updateItemTotal();
        }
    });

    removeBtn.addEventListener("click", () => {
        if (confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
            // item.remove();
            // updateCartTotal();
        }
    });

    updateItemTotal();
});
