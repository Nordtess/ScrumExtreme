document.addEventListener('DOMContentLoaded', function () {
    const form        = document.getElementById('createHatForm');
    if (!form) return;

    const submitBtn   = document.getElementById('hatSubmitBtn');
    const submitError = document.getElementById('hatSubmitError');

    const onlyLetters = /^[a-zA-ZåäöÅÄÖéèêëàâùûüîïôœæçÉÈÊËÀÂÙÛÜÎÏÔŒÆÇ,\s\-]+$/;

    const fieldRules = {
        Name: {
            validate: function (v) {
                if (!v.trim()) return 'Namn är obligatoriskt.';
                if (!onlyLetters.test(v.trim())) return 'Namnet får inte innehålla siffror.';
                return null;
            }
        },
        Price: {
            validate: function (v) {
                if (v.trim() === '') return 'Pris är obligatoriskt.';
                const num = parseFloat(v);
                if (isNaN(num) || num < 1) return 'Priset måste vara minst 1 kr.';
                return null;
            }
        }
    };

    function setError(id, msg) {
        const el = document.getElementById('error-' + id);
        if (el) el.textContent = msg || '';
    }

    function setInputState(input, state) {
        input.classList.remove('valid', 'invalid');
        if (state) input.classList.add(state);
    }

    function validateTextField(id) {
        const input = document.getElementById(id);
        if (!input) return true;
        const err = fieldRules[id].validate(input.value);
        setInputState(input, err ? 'invalid' : 'valid');
        setError(id, err);
        return !err;
    }

    function validateSizes() {
        const checked = form.querySelectorAll('input[name="Sizes"]:checked').length > 0;
        const errorEl = document.getElementById('error-Sizes');
        if (errorEl) errorEl.textContent = checked ? '' : 'Minst en storlek måste väljas.';
        const group = document.getElementById('sizesGroup');
        if (group) {
            group.style.outline = checked ? '' : '1px solid #ff453a';
            group.style.borderRadius = '6px';
        }
        return checked;
    }

    function validateMaterials() {
        const checked = form.querySelectorAll('input[name="SelectedMaterials"]:checked').length > 0;
        const errorEl = document.getElementById('error-Materials');
        if (errorEl) errorEl.textContent = checked ? '' : 'Minst ett material måste väljas.';
        const group = document.getElementById('materialsGroup');
        if (group) {
            group.style.outline = checked ? '' : '1px solid #ff453a';
            group.style.borderRadius = '6px';
        }
        return checked;
    }

    Object.keys(fieldRules).forEach(function (id) {
        const input = document.getElementById(id);
        if (!input) return;
        input.addEventListener('blur', function () {
            input.dataset.touched = '1';
            validateTextField(id);
        });
        input.addEventListener('input', function () {
            if (input.dataset.touched) {
                validateTextField(id);
            }
        });
        input.addEventListener('focus', function () {
            input.dataset.touched = '1';
        });
    });

    form.querySelectorAll('input[name="Sizes"]').forEach(function (cb) {
        cb.addEventListener('change', function () {
            validateSizes();
        });
    });

    form.querySelectorAll('input[name="SelectedMaterials"]').forEach(function (cb) {
        cb.addEventListener('change', function () {
            validateMaterials();
        });
    });

    function isFormValid() {
        const textOk      = Object.keys(fieldRules).every(function (id) {
            const input = document.getElementById(id);
            return input && input.classList.contains('valid');
        });
        const sizesOk     = form.querySelectorAll('input[name="Sizes"]:checked').length > 0;
        const materialsOk = form.querySelectorAll('input[name="SelectedMaterials"]:checked').length > 0;
        return textOk && sizesOk && materialsOk;
    }

    
    if (submitBtn) {
        submitBtn.addEventListener('mouseenter', function () {
            submitBtn.classList.remove('btn-hover-valid', 'btn-hover-invalid');
            submitBtn.classList.add(isFormValid() ? 'btn-hover-valid' : 'btn-hover-invalid');
        });
        submitBtn.addEventListener('mouseleave', function () {
            submitBtn.classList.remove('btn-hover-valid', 'btn-hover-invalid');
        });
    }

    form.addEventListener('submit', function (e) {
        Object.keys(fieldRules).forEach(function (id) {
            const input = document.getElementById(id);
            if (input) input.dataset.touched = '1';
        });
        const textValid      = Object.keys(fieldRules).map(validateTextField).every(Boolean);
        const sizesValid     = validateSizes();
        const materialsValid = validateMaterials();
        if (!textValid || !sizesValid || !materialsValid) {
            e.preventDefault();
            submitError.textContent = 'Rätta felen ovan innan du skickar.';
        } else {
            submitError.textContent = '';
        }
    });
});
