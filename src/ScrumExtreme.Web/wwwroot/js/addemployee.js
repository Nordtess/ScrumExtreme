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
});