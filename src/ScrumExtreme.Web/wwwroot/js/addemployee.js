document.addEventListener('DOMContentLoaded', function () {
    
    const form = document.getElementById('createAnstalldForm');
    if (!form) return;

    const submitBtn = document.getElementById('submitBtn');
    const submitError = document.getElementById('submitError');

    function toTitleCase(s) {
        return s.trim().toLowerCase().replace(/(?:^|\s)\S/g, function (ch) {
            return ch.toUpperCase();
        });
    }

    function normalizeEmail(s) {
        return s.trim().toLowerCase();
    }

    const onlyLetters = /^[a-zA-ZåäöÅÄÖéèêëàâùûüîïôœæçÉÈÊËÀÂÙÛÜÎÏÔŒÆÇ\s\-]+$/;

    const fieldRules = [
        {
            id: 'FirstName',
            normalize: toTitleCase,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Förnamn måste vara ifyllt';
                if (!onlyLetters.test(t)) return 'Endast bokstäver är tillåtna';
                return null;
            }
        },
        {
            id: 'LastName',
            normalize: toTitleCase,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Efternamn måste vara ifyllt';
                if (!onlyLetters.test(t)) return 'Endast bokstäver är tillåtna';
                return null;
            }
        },
        {
            id: 'Email',
            normalize: normalizeEmail,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'E-post måste vara ifyllt';
                if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(t))
                    return 'Din e-post måste vara skriven som abc@mail.com';
                return null;
            }
        },
        {
            id: 'Username',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Användarnamn måste vara ifyllt';
                if (t.length < 3) return 'Minst 3 tecken';
                return null;
            }
        },
        {
            id: 'Password',
            validate: function (v) {
                if (!v) return 'Lösenord måste vara ifyllt';
                if (v.length < 6) return 'Lösenordet måste vara minst 6 tecken';
                return null;
            }
        },
        {
            id: 'Address',
            normalize: toTitleCase,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Adress måste vara ifyllt';
                return null;
            }
        },
        {
            id: 'City',
            normalize: toTitleCase,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Stad måste vara ifyllt';
                if (!onlyLetters.test(t)) return 'Endast bokstäver är tillåtna';
                return null;
            }
        },
        {
            id: 'PostalCode',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Postnummer måste vara ifyllt';
                if (!/^\d{3,10}$/.test(t)) return 'Postnummer får bara innehålla siffror';
                return null;
            }
        },
        {
            id: 'PhoneNumber',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Telefonnummer måste vara ifyllt';
                if (!/^[\d\+\-\s]+$/.test(t)) return 'Ange ett giltigt telefonnummer';
                return null;
            }
        },
        {
            id: 'Role',
            validate: function (v) {
                if (v !== 'employee' && v !== 'admin') {
                    return 'Ogiltig behörighetsnivå vald';
                }
                return null;
            }
        },
    ];

    fieldRules.forEach(function (rule) {
        const input = document.getElementById(rule.id);
        if (!input) return;

        input.addEventListener('blur', function () {
            if (rule.normalize) this.value = rule.normalize(this.value);
            this.dataset.touched = '1';
            runRuleValidation(rule);
        });

        input.addEventListener('input', function () {
            if (this.dataset.touched) runRuleValidation(rule);
        });
    });

    submitBtn.addEventListener('mouseenter', function () {
        if (isFormValid()) {
            submitBtn.classList.add('btn-hover-valid');
            submitBtn.classList.remove('btn-hover-invalid');
            submitError.textContent = '';
        } else {
            submitBtn.classList.add('btn-hover-invalid');
            submitBtn.classList.remove('btn-hover-valid');
            submitError.textContent = 'Alla fält måste vara korrekt ifyllda';
        }
    });

    submitBtn.addEventListener('mouseleave', function () {
        submitBtn.classList.remove('btn-hover-valid', 'btn-hover-invalid');
        submitError.textContent = '';
    });

    form.addEventListener('submit', function (e) {
        let hasError = false;
        fieldRules.forEach(function (rule) {
            const input = document.getElementById(rule.id);
            if (!input) return;
            if (rule.normalize) input.value = rule.normalize(input.value);
            if (runRuleValidation(rule)) hasError = true;
        });

        if (hasError) {
            e.preventDefault();
            submitError.textContent = 'Alla fält måste vara korrekt ifyllda';
        }
    });

    function runRuleValidation(rule) {
        const input = document.getElementById(rule.id);
        if (!input) return false;
        input.dataset.touched = '1';
        const err = rule.validate(input.value);
        setInputState(input, err ? 'invalid' : 'valid');
        setError(rule.id, err || '');
        return !!err;
    }

    function isFormValid() {
        return fieldRules.every(function (r) {
            const input = document.getElementById(r.id);
            if (!input) return false;
            return r.validate(input.value) === null;
        });
    }

    function setInputState(input, state) {
        input.classList.remove('valid', 'invalid');
        if (state) input.classList.add(state);
    }

    function setError(id, msg) {
        const el = document.getElementById('error-' + id);
        if (el) el.textContent = msg;
    }

    // ── Random password generator ─────────────────────────────────────────
    const genPwCheck    = document.getElementById('genPwCheck');
    const passwordInput = document.getElementById('passwordInput');

    function generatePassword() {
        const upper   = 'ABCDEFGHJKLMNPQRSTUVWXYZ';
        const lower   = 'abcdefghjkmnpqrstuvwxyz';
        const digits  = '23456789';
        const special = '!@#%&*';
        const all = upper + lower + digits + special;
        const arr = new Uint8Array(12);
        crypto.getRandomValues(arr);
        let pw = upper[arr[0] % upper.length]
               + lower[arr[1] % lower.length]
               + digits[arr[2] % digits.length]
               + special[arr[3] % special.length];
        for (let i = 4; i < 12; i++) pw += all[arr[i] % all.length];
        const shuffled = new Uint8Array(12);
        crypto.getRandomValues(shuffled);
        return pw.split('').sort((a, b) => shuffled[pw.indexOf(a)] - shuffled[pw.indexOf(b)]).join('');
    }

    if (genPwCheck && passwordInput) {
        genPwCheck.addEventListener('change', function () {
            if (this.checked) {
                const pw = generatePassword();
                passwordInput.value    = pw;
                passwordInput.type     = 'text';
                passwordInput.readOnly = true;
                passwordInput.style.background    = '#fffdf5';
                passwordInput.style.borderColor   = '#c9a84c';
                passwordInput.style.fontFamily    = 'monospace';
                passwordInput.style.letterSpacing = '0.04em';
            } else {
                passwordInput.value    = '';
                passwordInput.type     = 'password';
                passwordInput.readOnly = false;
                passwordInput.style.background    = '';
                passwordInput.style.borderColor   = '';
                passwordInput.style.fontFamily    = '';
                passwordInput.style.letterSpacing = '';
            }
        });
    }
});