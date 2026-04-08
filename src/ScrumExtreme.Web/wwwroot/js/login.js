document.addEventListener('DOMContentLoaded', function () {
    const btn      = document.getElementById('loginBtn');
    const username = document.getElementById('username');
    const password = document.getElementById('password');

    if (!btn || !username || !password) return;

    function bothFilled() {
        return username.value.trim().length > 0 && password.value.trim().length > 0;
    }

    btn.addEventListener('mouseenter', function () {
        if (bothFilled()) {
            btn.classList.add('btn-hover-valid');
            btn.classList.remove('btn-hover-invalid');
        } else {
            btn.classList.add('btn-hover-invalid');
            btn.classList.remove('btn-hover-valid');
        }
    });

    btn.addEventListener('mouseleave', function () {
        btn.classList.remove('btn-hover-valid', 'btn-hover-invalid');
    });
});
