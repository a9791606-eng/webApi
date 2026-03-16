async function doLogin() {
    const u = document.getElementById('username').value.trim();
    const p = document.getElementById('password').value.trim();
    const err = document.getElementById('error');
    err.innerText = '';
    if (!u || !p) { err.innerText = 'Please fill username and password'; return; }

    try {
        const response = await fetch('/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username: u, Password: p })
        });
        if (!response.ok) {
            err.innerText = 'Invalid credentials';
            return;
        }
        const token = await response.text();
        // store token
        sessionStorage.setItem('icecream_token', token);
        // redirect to main page
        window.location.href = '/index.html';
    } catch (e) {
        console.error(e);
        err.innerText = 'Network error';
    }
}
