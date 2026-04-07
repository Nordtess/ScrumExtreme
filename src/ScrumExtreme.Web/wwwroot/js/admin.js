document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createCustomerForm');
    if (!form) return;

    const submitBtn  = document.getElementById('submitBtn');
    const submitError = document.getElementById('submitError');

    // Phone library loaded via CDN — global is window.libphonenumber
    const phoneLib = window.libphonenumber || null;

    // ── Country picker ───────────────────────────────────────────────────
    let countries      = [];
    let selectedCountry = null;   // { name, iso, dial }
    let currentMatches = [];
    let highlightedIdx = -1;

    const countrySearch   = document.getElementById('countrySearch');
    const countryHidden   = document.getElementById('countryHidden');
    const countryDropdown = document.getElementById('countryDropdown');

    fetch('/js/countries.json')
        .then(function (r) { return r.json(); })
        .then(function (data) { countries = data; })
        .catch(function () { console.warn('[admin] countries.json not loaded'); });

    countrySearch.addEventListener('input', function () {
        const q = this.value.trim().toLowerCase();
        selectedCountry     = null;
        countryHidden.value = '';
        if (!q) { closeDropdown(); return; }

        const starts   = countries.filter(function (c) { return c.name.toLowerCase().startsWith(q); });
        const contains = countries.filter(function (c) { return !c.name.toLowerCase().startsWith(q) && c.name.toLowerCase().includes(q); });
        renderDropdown(starts.concat(contains).slice(0, 8));
    });

    countrySearch.addEventListener('keydown', function (e) {
        if (countryDropdown.classList.contains('country-dropdown--hidden')) return;
        if (e.key === 'ArrowDown') {
            e.preventDefault();
            highlightedIdx = Math.min(highlightedIdx + 1, currentMatches.length - 1);
            updateHighlight();
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            highlightedIdx = Math.max(highlightedIdx - 1, 0);
            updateHighlight();
        } else if (e.key === 'Enter') {
            e.preventDefault();
            if (highlightedIdx >= 0) selectCountry(currentMatches[highlightedIdx]);
        } else if (e.key === 'Escape') {
            closeDropdown();
        }
    });

    countrySearch.addEventListener('blur', function () {
        setTimeout(function () {
            closeDropdown();
            validateCountryField();
        }, 160);
    });

    function renderDropdown(matches) {
        currentMatches = matches;
        highlightedIdx = -1;
        countryDropdown.innerHTML = '';
        matches.forEach(function (c) {
            const li = document.createElement('li');
            li.className = 'country-option';
            li.textContent = c.name + '  ' + c.dial;
            li.addEventListener('mousedown', function (e) {
                e.preventDefault();
                selectCountry(c);
            });
            countryDropdown.appendChild(li);
        });
        countryDropdown.classList.remove('country-dropdown--hidden');
    }

    function updateHighlight() {
        countryDropdown.querySelectorAll('.country-option').forEach(function (li, i) {
            li.classList.toggle('country-option--active', i === highlightedIdx);
        });
    }

    function selectCountry(c) {
        if (!c) return;
        selectedCountry     = c;
        countrySearch.value = c.name;
        countryHidden.value = c.name;
        closeDropdown();

        // Auto-fill phone prefix if phone field is empty or only holds a dial code
        const phone = document.getElementById('PhoneNumber');
        if (phone) {
            const cur = phone.value.trim();
            if (!cur || /^\+\d{0,4}$/.test(cur)) {
                phone.value = c.dial;
                phone.setSelectionRange(phone.value.length, phone.value.length);
            }
        }

        countrySearch.dataset.touched = '1';
        setInputState(countrySearch, 'valid');
        setError('Country', '');
    }

    function closeDropdown() {
        countryDropdown.classList.add('country-dropdown--hidden');
    }

    function validateCountryField() {
        countrySearch.dataset.touched = '1';
        if (!countryHidden.value) {
            const msg = countrySearch.value.trim()
                ? 'Välj ett land från listan'
                : 'Land måste väljas';
            setInputState(countrySearch, 'invalid');
            setError('Country', msg);
            return true;
        }
        setInputState(countrySearch, 'valid');
        setError('Country', '');
        return false;
    }

    // ── Regular field rules ──────────────────────────────────────────────
    const onlyLetters = /^[a-zA-ZåäöÅÄÖéèêëàâùûüîïôœæçÉÈÊËÀÂÙÛÜÎÏÔŒÆÇ\s\-]+$/;

    const fieldRules = [
        {
            id: 'FirstName',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Förnamn måste vara ifyllt';
                if (!onlyLetters.test(t)) return 'Endast bokstäver är tillåtna';
                return null;
            }
        },
        {
            id: 'LastName',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Efternamn måste vara ifyllt';
                if (!onlyLetters.test(t)) return 'Endast bokstäver är tillåtna';
                return null;
            }
        },
        {
            id: 'Address',
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Adress måste vara ifyllt';
                if (!/[a-zA-ZåäöÅÄÖ]/.test(t)) return 'Ange gatunamn och husnummer, t.ex. Strandvägen 13';
                if (!/\d/.test(t)) return 'Ange gatunamn och husnummer, t.ex. Strandvägen 13';
                return null;
            }
        },
        {
            id: 'City',
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
            normalize: normalizePhone,
            validate: validatePhone
        }
    ];

    // Attach blur and input listeners
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

    // Button hover
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

    // Form submit
    form.addEventListener('submit', function (e) {
        let hasError = false;
        fieldRules.forEach(function (rule) {
            const input = document.getElementById(rule.id);
            if (!input) return;
            if (rule.normalize) input.value = rule.normalize(input.value);
            if (runRuleValidation(rule)) hasError = true;
        });
        if (validateCountryField()) hasError = true;
        if (hasError) {
            e.preventDefault();
            submitError.textContent = 'Alla fält måste vara korrekt ifyllda';
        }
    });

    // Returns true if field has an error
    function runRuleValidation(rule) {
        const input = document.getElementById(rule.id);
        if (!input) return false;
        input.dataset.touched = '1';
        const err = rule.validate(input.value);
        setInputState(input, err ? 'invalid' : 'valid');
        setError(rule.id, err || '');
        return !!err;
    }

    // ── Phone (libphonenumber-js + fallback) ─────────────────
    function normalizePhone(value) {
        const trimmed = value.trim();
        if (!trimmed) return trimmed;
        const iso = selectedCountry ? selectedCountry.iso : 'SE';
        if (phoneLib) {
            try {
                const parsed = phoneLib.parsePhoneNumber(trimmed, iso);
                if (parsed && parsed.isValid()) return parsed.format('E.164');
            } catch (e) { /* fall through to fallback */ }
        }
        return fallbackNormalize(trimmed);
    }

    function validatePhone(value) {
        const trimmed = value.trim();
        if (!trimmed) return 'Telefonnummer måste vara ifyllt';
        const iso = selectedCountry ? selectedCountry.iso : 'SE';
        if (phoneLib) {
            try {
                const parsed = phoneLib.parsePhoneNumber(trimmed, iso);
                if (parsed && parsed.isValid()) return null;
            } catch (e) { /* fall through */ }
            return 'Ogiltigt telefonnummer';
        }
        if (!/^\+\d{7,15}$/.test(trimmed)) return 'Ogiltigt format, t.ex. +46701234567';
        return null;
    }

    function fallbackNormalize(s) {
        let r = s.replace(/[^\d+]/g, '');
        if (r.startsWith('+')) return r;
        r = r.replace(/\D/g, '');
        if (r.startsWith('0')) return '+46' + r.slice(1);
        if (r.startsWith('46')) return '+' + r;
        return '+46' + r;
    }

    // ── Helpers ──────────────────────────────────────────────
    function isFormValid() {
        const rulesOk = fieldRules.every(function (r) {
            return r.validate(document.getElementById(r.id).value) === null;
        });
        return rulesOk && !!countryHidden.value;
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
