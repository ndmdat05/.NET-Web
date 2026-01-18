// ================= TAB NAVIGATION =================
const tabLinks = document.querySelectorAll('.tab-link');
const sections = document.querySelectorAll('.section');

tabLinks.forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();

        // Remove active class from all links
        tabLinks.forEach(l => l.classList.remove('active'));
        link.classList.add('active');

        // Show corresponding section
        const targetId = link.getAttribute('href').replace('#', '');
        sections.forEach(section => {
            section.classList.remove('active');
            if (section.id === targetId) {
                section.classList.add('active');
            }
        });
    });
});

// ================= SIDEBAR TOGGLE =================
const menuToggle = document.getElementById('menuToggle');
const sidebar = document.querySelector('.sidebar');

if (menuToggle) {
    menuToggle.addEventListener('click', () => {
        sidebar.classList.toggle('collapsed');
        sidebar.classList.toggle('show');
    });
}

// ================= MODAL FUNCTIONS =================
function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('show');
        document.body.style.overflow = '';
    }
}

// Close modal when clicking outside
document.addEventListener('click', (e) => {
    if (e.target.classList.contains('modal')) {
        e.target.classList.remove('show');
        document.body.style.overflow = '';
    }
});

// ================= SELECT ALL CHECKBOX =================
const selectAll = document.getElementById('selectAll');
if (selectAll) {
    selectAll.addEventListener('change', function () {
        const checkboxes = document.querySelectorAll('. admin-table tbody input[type="checkbox"]');
        checkboxes.forEach(cb => cb.checked = this.checked);
    });
}

// ================= STATUS SELECT COLOR =================
const statusSelects = document.querySelectorAll('.status-select');
statusSelects.forEach(select => {
    updateStatusColor(select);
    select.addEventListener('change', function () {
        updateStatusColor(this);
    });
});

function updateStatusColor(select) {
    select.className = 'status-select ' + select.value;
}

// ================= SEARCH FUNCTIONALITY =================
const productSearch = document.getElementById('productSearch');
if (productSearch) {
    productSearch.addEventListener('input', function () {
        const searchTerm = this.value.toLowerCase();
        const rows = document.querySelectorAll('.admin-table tbody tr');

        rows.forEach(row => {
            const productName = row.querySelector('td:nth-child(3)')?.textContent.toLowerCase();
            if (productName && productName.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    });
}

// ================= DELETE CONFIRMATION =================
document.querySelectorAll('.btn-delete').forEach(btn => {
    btn.addEventListener('click', function () {
        if (confirm('B?n có ch?c ch?n mu?n xóa m?c này?')) {
            // Handle delete
            const row = this.closest('tr');
            row.style.animation = 'fadeOut 0.3s ease';
            setTimeout(() => row.remove(), 300);
        }
    });
});

// ================= IMAGE UPLOAD PREVIEW =================
const imageUpload = document.querySelector('.image-upload input[type="file"]');
if (imageUpload) {
    imageUpload.addEventListener('change', function () {
        const placeholder = this.parentElement.querySelector('.upload-placeholder');
        if (this.files.length > 0) {
            placeholder.innerHTML = `
                <i class="fas fa-check-circle" style="color: #28a745;"></i>
                <p>${this.files.length} ?nh ?ã ???c ch?n</p>
            `;
        }
    });
}

// ================= NOTIFICATION DROPDOWN =================
const notification = document.querySelector('.notification');
if (notification) {
    notification.addEventListener('click', function () {
        // Toggle notification dropdown
        alert('B?n có 3 thông báo m?i! ');
    });
}

// ================= FORM SUBMISSION =================
const addProductForm = document.getElementById('addProductForm');
if (addProductForm) {
    addProductForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Validate form
        const formData = new FormData(this);

        // Show success message
        alert('S?n ph?m ?ã ???c thêm thành công!');
        closeModal('addProductModal');
        this.reset();
    });
}

// ================= ANIMATIONS =================
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeOut {
        from { opacity: 1; transform: translateX(0); }
        to { opacity:  0; transform:  translateX(-20px); }
    }
`;
document.head.appendChild(style);

// ================= INITIALIZE TOOLTIPS =================
document.querySelectorAll('[title]').forEach(el => {
    el.addEventListener('mouseenter', function () {
        // Simple tooltip logic
    });
});

console.log('Admin Panel Loaded Successfully!  ??');