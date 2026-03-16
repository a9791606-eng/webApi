async function doSignup() {
    const username = document.getElementById('username').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();
    const confirmPassword = document.getElementById('confirmPassword').value.trim();
    const branch = document.getElementById('branch').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const errorEl = document.getElementById('error');
    
    errorEl.style.display = 'none';
    errorEl.innerText = '';
    
    // וידוות הפחות
    if (!username || !email || !password || !confirmPassword || !branch) {
        errorEl.innerText = 'אנא מלא את כל השדות החובה (שם משתמש, אימייל, סיסמה, שם סניף)';
        errorEl.style.display = 'block';
        return;
    }
    
    if (!email.includes('@')) {
        errorEl.innerText = 'אנא הכנס אימייל תקין';
        errorEl.style.display = 'block';
        return;
    }
    
    if (password.length < 3) {
        errorEl.innerText = 'הסיסמה חייבת להיות לפחות 3 תווים';
        errorEl.style.display = 'block';
        return;
    }
    
    if (password !== confirmPassword) {
        errorEl.innerText = 'הסיסמאות אינן תואמות';
        errorEl.style.display = 'block';
        return;
    }
    
    try {
        // שליחת בקשת signup לשרת
        const response = await fetch('/Users/signup', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                Username: username,
                Email: email,
                Password: password,
                Branch: branch,
                Phone: phone,
                IsAdmin: false
            })
        });
        
        if (!response.ok) {
            const errorData = await response.text();
            errorEl.innerText = errorData || 'שגיאה בהרשמה. אנא נסה שוב.';
            errorEl.style.display = 'block';
            return;
        }
        
        // בהצלחה - התחבר ישירות
        const loginResponse = await fetch('/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username: username, Password: password })
        });
        
        if (!loginResponse.ok) {
            errorEl.innerText = 'נרשמת בהצלחה! אנא התחבר בעמוד ההתחברות.';
            errorEl.style.display = 'block';
            setTimeout(() => {
                window.location.href = '/login.html';
            }, 2000);
            return;
        }
        
        const token = await loginResponse.text();
        sessionStorage.setItem('icecream_token', token);
        window.location.href = '/index.html';
        
    } catch (e) {
        console.error(e);
        errorEl.innerText = 'שגיאת רשת. אנא נסה שוב.';
        errorEl.style.display = 'block';
    }
}
