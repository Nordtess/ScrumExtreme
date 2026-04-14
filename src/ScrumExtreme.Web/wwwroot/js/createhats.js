document.addEventListener('DOMContentLoaded', function () {
    const form        = document.getElementById('createHatForm');
    if (!form) return;

    const submitBtn   = document.getElementById('hatSubmitBtn');
    const submitError = document.getElementById('hatSubmitError');

    // ── Validation rules ─────────────────────────────────────────────────
    const rules = {
        Name: {
            required: true,
            pattern: /^[^\d]+$/,
            messages: {
                required: 'Namn är obligatoriskt.',
                pattern:  'Namnet får inte innehålla siffror.'
            }
        },
        Size: {
            required: true,
            pattern: /^[^\d]+$/,
            messages: {
                required: 'Storlek är obligatoriskt.',
                pattern:  'Storlek får inte innehålla siffror.'
            }
        },
        Price: {
            required: true,
            min: 0,
            messages: {
                required: 'Pris är obligatoriskt.',
                min:      'Priset kan inte vara negativt.'
            }
        },
        MaterialList: {
            required: true,
            pattern: /^[^\d]+$/,
            messages: {
                required: 'Materiallista är obligatorisk.',
                pattern:  'Materiallistan får inte innehålla siffror.'
            }
        }
    };

    // ── Helpers ───────────────────────────────────────────────────────────
    function setError(fieldId, msg) {
        const el = document.getElementById('error-' + fieldId);
        if (el) el.textContent = msg;
    }

    function setInputState(input, state) {
        input.classList.remove('valid', 'invalid');
        if (state) input.classList.add(state);
    }

    function validateField(fieldId) {
        const input = document.getElementById(fieldId);
        if (!input) return true;

        const rule = rules[fieldId];
        const value = input.value.trim();

        if (rule.required && value === '') {
            setError(fieldId, rule.messages.required);
            setInputState(input, 'invalid');
            return false;
        }

        if (fieldId === 'Price') {
            const num = parseFloat(value);
            if (isNaN(num) || num < rule.min) {
                setError(fieldId, rule.messages.min);
                setInputState(input, 'invalid');
                return false;
            }
        }

        if (rule.pattern && !rule.pattern.test(value)) {
            setError(fieldId, rule.messages.pattern);
            setInputState(input, 'invalid');
            return false;
        }

        setError(fieldId, '');
        setInputState(input, 'valid');
        return true;
    }

    // ── Attach blur listeners ─────────────────────────────────────────────
    Object.keys(rules).forEach(function (fieldId) {
        const input = document.getElementById(fieldId);
        if (!input) return;
        input.addEventListener('blur', function () {
            validateField(fieldId);
            updateSubmitBtn();
        });
        input.addEventListener('input', function () {
            if (input.dataset.touched) {
                validateField(fieldId);
                updateSubmitBtn();
            }
        });
        input.addEventListener('focus', function () {
            input.dataset.touched = '1';
        });
    });

    // ── Submit button hover state ─────────────────────────────────────────
    function isFormValid() {
        return Object.keys(rules).every(function (fieldId) {
            const input = document.getElementById(fieldId);
            return input && input.classList.contains('valid');
        });
    }

    function updateSubmitBtn() {
        submitBtn.classList.remove('btn-hover-valid', 'btn-hover-invalid');
        if (isFormValid()) {
            submitBtn.classList.add('btn-hover-valid');
        } else {
            submitBtn.classList.add('btn-hover-invalid');
        }
    }

    // ── Form submit ───────────────────────────────────────────────────────
    form.addEventListener('submit', function (e) {
        const allValid = Object.keys(rules).map(validateField).every(Boolean);
        if (!allValid) {
            e.preventDefault();
            submitError.textContent = 'Rätta felen ovan innan du skickar.';
            updateSubmitBtn();
        } else {
            submitError.textContent = '';
        }
    });
});
