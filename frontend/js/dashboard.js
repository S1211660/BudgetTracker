const API = 'http://localhost:7298/api';
const token = localStorage.getItem('token');
if (!token) window.location.href = 'index.html';

let currentMonth = new Date().getMonth() + 1;
let currentYear = new Date().getFullYear();
let categoryChart = null;

function authHeaders() {
    return { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' };
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = 'index.html';
}

function formatEuro(amount) {
    return '€' + amount.toFixed(2).replace('.', ',');
}

function changeMonth(direction) {
    currentMonth += direction;
    if (currentMonth > 12) { currentMonth = 1; currentYear++; }
    if (currentMonth < 1) { currentMonth = 12; currentYear--; }
    loadAll();
}

function updateMonthLabel() {
    const months = ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni',
        'Juli', 'Augustus', 'September', 'Oktober', 'November', 'December'];
    document.getElementById('current-month').textContent = `${months[currentMonth - 1]} ${currentYear}`;
}

async function loadSummary() {
    const res = await fetch(`${API}/summary?month=${currentMonth}&year=${currentYear}`, {
        headers: authHeaders()
    });
    const data = await res.json();

    document.getElementById('total-income').textContent = formatEuro(data.totalIncome);
    document.getElementById('total-expense').textContent = formatEuro(data.totalExpense);
    document.getElementById('total-balance').textContent = formatEuro(data.balance);

    // Grafiek
    const labels = data.byCategory.map(c => c.category);
    const values = data.byCategory.map(c => c.total);
    const colors = data.byCategory.map(c => c.type === 'income' ? '#22c55e' : '#ef4444');

    if (categoryChart) categoryChart.destroy();
    const ctx = document.getElementById('categoryChart').getContext('2d');
    categoryChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: 'Bedrag (€)',
                data: values,
                backgroundColor: colors,
                borderRadius: 6
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
}

async function loadCategories() {
    const res = await fetch(`${API}/categories`, { headers: authHeaders() });
    const categories = await res.json();

    const select = document.getElementById('t-category');
    select.innerHTML = categories.map(c =>
        `<option value="${c.id}">${c.name} (${c.type === 'income' ? 'inkomsten' : 'uitgaven'})</option>`
    ).join('');
}

async function loadTransactions() {
    const res = await fetch(`${API}/transactions?month=${currentMonth}&year=${currentYear}`, {
        headers: authHeaders()
    });
    const transactions = await res.json();

    const tbody = document.getElementById('transactions-list');
    tbody.innerHTML = transactions.map(t => `
        <tr>
            <td>${new Date(t.date).toLocaleDateString('nl-NL')}</td>
            <td>${t.description}</td>
            <td><span class="badge ${t.categoryType}">${t.categoryName}</span></td>
            <td style="color: ${t.categoryType === 'income' ? '#22c55e' : '#ef4444'}; font-weight: 600;">
                ${t.categoryType === 'income' ? '+' : '-'}${formatEuro(t.amount)}
            </td>
            <td><button class="delete-btn" onclick="deleteTransaction(${t.id})">🗑</button></td>
        </tr>
    `).join('');
}

async function addTransaction() {
    const categoryId = parseInt(document.getElementById('t-category').value);
    const amount = parseFloat(document.getElementById('t-amount').value);
    const description = document.getElementById('t-description').value;
    const date = document.getElementById('t-date').value;

    if (!categoryId || !amount || !date) {
        document.getElementById('t-error').textContent = 'Vul alle velden in.';
        return;
    }

    const res = await fetch(`${API}/transactions`, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify({ categoryId, amount, description, date })
    });

    if (res.ok) {
        document.getElementById('t-amount').value = '';
        document.getElementById('t-description').value = '';
        document.getElementById('t-error').textContent = '';
        loadAll();
    } else {
        document.getElementById('t-error').textContent = 'Er ging iets mis.';
    }
}

async function deleteTransaction(id) {
    await fetch(`${API}/transactions/${id}`, {
        method: 'DELETE',
        headers: authHeaders()
    });
    loadAll();
}

async function addCategory() {
    const name = document.getElementById('c-name').value;
    const type = document.getElementById('c-type').value;

    if (!name) {
        document.getElementById('c-error').textContent = 'Vul een naam in.';
        return;
    }

    const res = await fetch(`${API}/categories`, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify({ name, type })
    });

    if (res.ok) {
        document.getElementById('c-name').value = '';
        document.getElementById('c-error').textContent = '';
        loadCategories();
    } else {
        document.getElementById('c-error').textContent = 'Er ging iets mis.';
    }
}

// Gebruikersnaam uit token halen
function loadUsername() {
    const payload = JSON.parse(atob(token.split('.')[1]));
    document.getElementById('nav-username').textContent = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
}

function loadAll() {
    updateMonthLabel();
    loadSummary();
    loadCategories();
    loadTransactions();
}

loadUsername();
loadAll();