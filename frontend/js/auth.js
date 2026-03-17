const API = 'https://localhost:7298/api';

function showTab(tab) {
    document.getElementById('login').classList.add('hidden');
    document.getElementById('register').classList.add('hidden');
    document.getElementById(tab).classList.remove('hidden');
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    event.target.classList.add('active');
}

async function login() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;

    const res = await fetch(`${API}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
    });

    if (res.ok) {
        const data = await res.json();
        localStorage.setItem('token', data.token);
        window.location.href = 'dashboard.html';
    } else {
        document.getElementById('login-error').textContent = 'Ongeldige inloggegevens.';
    }
}

async function register() {
    const username = document.getElementById('reg-username').value;
    const email = document.getElementById('reg-email').value;
    const password = document.getElementById('reg-password').value;

    const res = await fetch(`${API}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email, password })
    });

    if (res.ok) {
        document.getElementById('reg-success').textContent = 'Registratie gelukt! Je kunt nu inloggen.';
        document.getElementById('reg-error').textContent = '';
    } else {
        const msg = await res.text();
        document.getElementById('reg-error').textContent = msg;
        document.getElementById('reg-success').textContent = '';
    }
}