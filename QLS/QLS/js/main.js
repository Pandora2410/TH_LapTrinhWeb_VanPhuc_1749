const API_BASE_URL = 'https://localhost:7170/api/products';
let selectedProductId = null;

document.addEventListener('DOMContentLoaded', function () {
    fetchProducts();

    document.getElementById('studentForm').addEventListener('submit', function (e) {
        e.preventDefault();
    });

    document.getElementById('btnAdd').addEventListener('click', addProduct);
    document.getElementById('btnUpdate').addEventListener('click', updateProduct);
    document.getElementById('btnReset').addEventListener('click', resetForm);

    document.getElementById('productList').addEventListener('click', function (e) {
        const button = e.target.closest('button');
        if (!button) return;

        const id = button.dataset.id;

        if (button.classList.contains('delete-btn')) {
            deleteProduct(id);
        }

        if (button.classList.contains('edit-btn')) {
            loadProductToForm(id);
        }

        if (button.classList.contains('view-btn')) {
            viewProduct(id);
        }
    });
});

async function fetchProducts() {
    try {
        const response = await fetch(API_BASE_URL);
        const products = await handleResponse(response);
        displayProducts(products);
    } catch (error) {
        console.error('Fetch error:', error);
        alert('Không tải được danh sách sách. Kiểm tra API URL, HTTPS certificate hoặc CORS.');
    }
}

async function handleResponse(response) {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || `HTTP ${response.status}`);
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

function displayProducts(products) {
    const productList = document.getElementById('productList');
    productList.innerHTML = '';

    if (!products || products.length === 0) {
        productList.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">Chưa có sách nào</td>
            </tr>
        `;
        return;
    }

    products.forEach(product => {
        productList.innerHTML += createProductRow(product);
    });
}

function createProductRow(product) {
    return `
        <tr>
            <td>${product.id}</td>
            <td>${product.name ?? ''}</td>
            <td>${product.price ?? ''}</td>
            <td>${product.description ?? ''}</td>
            <td>
                <button type="button" class="btn btn-danger btn-sm delete-btn" data-id="${product.id}">Delete</button>
                <button type="button" class="btn btn-warning btn-sm edit-btn text-white" data-id="${product.id}">Edit</button>
                <button type="button" class="btn btn-primary btn-sm view-btn" data-id="${product.id}">View</button>
            </td>
        </tr>
    `;
}

async function addProduct() {
    const productData = getFormData();

    if (!productData.name || productData.price === null) {
        alert('Vui lòng nhập tên sách và giá hợp lệ.');
        return;
    }

    try {
        await fetch(API_BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        }).then(handleResponse);

        alert('Thêm sách thành công.');
        resetForm();
        fetchProducts();
    } catch (error) {
        console.error('Add error:', error);
        alert('Thêm sách thất bại. Kiểm tra Console/F12 để xem lỗi chi tiết.');
    }
}

async function loadProductToForm(id) {
    try {
        const product = await fetch(`${API_BASE_URL}/${id}`).then(handleResponse);

        selectedProductId = product.id;
        document.getElementById('bookName').value = product.name ?? '';
        document.getElementById('price').value = product.price ?? '';
        document.getElementById('description').value = product.description ?? '';
    } catch (error) {
        console.error('Load product error:', error);
        alert('Không lấy được thông tin sách.');
    }
}

async function updateProduct() {
    if (!selectedProductId) {
        alert('Vui lòng chọn Edit một sách trước khi cập nhật.');
        return;
    }

    const productData = {
        id: Number(selectedProductId),
        ...getFormData()
    };

    try {
        await fetch(`${API_BASE_URL}/${selectedProductId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        }).then(handleResponse);

        alert('Cập nhật sách thành công.');
        resetForm();
        fetchProducts();
    } catch (error) {
        console.error('Update error:', error);
        alert('Cập nhật thất bại. Kiểm tra API có nhận đúng id/name/price/description không.');
    }
}

async function deleteProduct(id) {
    if (!confirm('Bạn có chắc muốn xóa sách này không?')) return;

    try {
        await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        }).then(handleResponse);

        alert('Xóa sách thành công.');
        fetchProducts();
    } catch (error) {
        console.error('Delete error:', error);
        alert('Xóa thất bại.');
    }
}

async function viewProduct(id) {
    try {
        const product = await fetch(`${API_BASE_URL}/${id}`).then(handleResponse);

        document.querySelectorAll('[data-atr="id"]').forEach(el => el.textContent = product.id);
        document.querySelectorAll('[data-atr="bookname"]').forEach(el => el.textContent = product.name ?? '');
        document.querySelectorAll('[data-atr="description"]').forEach(el => el.textContent = product.description ?? '');

        const modal = new bootstrap.Modal(document.getElementById('modalViewDetailInfo'));
        modal.show();
    } catch (error) {
        console.error('View error:', error);
        alert('Không xem được chi tiết sách.');
    }
}

function getFormData() {
    return {
        name: document.getElementById('bookName').value.trim(),
        price: Number(document.getElementById('price').value),
        description: document.getElementById('description').value.trim()
    };
}

function resetForm() {
    selectedProductId = null;
    document.getElementById('bookName').value = '';
    document.getElementById('price').value = '';
    document.getElementById('description').value = '';
}
