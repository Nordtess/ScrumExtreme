document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createCustomerForm');
    if (!form) return;

    const submitBtn  = document.getElementById('submitBtn');
    const submitError = document.getElementById('submitError');

    // ── Country picker ───────────────────────────────────────────────────
    let countries         = [];
    let selectedCountry   = null;   // { name, iso, dial, lang }
    let currentMatches    = [];
    let highlightedIdx    = -1;
    let currentDialPrefix = '';     // e.g. "+46" – locked into phone field

    const countrySearch     = document.getElementById('countrySearch');
    const countryHidden     = document.getElementById('countryHidden');
    const countryCodeHidden = document.getElementById('countryCodeHidden');
    const countryDropdown   = document.getElementById('countryDropdown');

    fetch('/js/countries.json')
        .then(function (r) { return r.json(); })
        .then(function (data) { countries = data; })
        .catch(function () { console.warn('[admin] countries.json not loaded'); });

    countrySearch.addEventListener('input', function () {
        const q = this.value.trim().toLowerCase();
        selectedCountry        = null;
        currentDialPrefix      = '';
        countryHidden.value    = '';
        if (countryCodeHidden) countryCodeHidden.value = '';
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
        selectedCountry       = c;
        countrySearch.value   = c.name;
        countryHidden.value   = c.name;
        if (countryCodeHidden) countryCodeHidden.value = c.iso;
        closeDropdown();

        // Lock phone field to country dial code prefix
        const phone = document.getElementById('PhoneNumber');
        if (phone) {
            currentDialPrefix  = c.dial;
            phone.value        = c.dial;
            phone.focus();
            phone.setSelectionRange(phone.value.length, phone.value.length);
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

    // ── Normalizers ───────────────────────────────────────────────────────
    function toTitleCase(s) {
        return s.trim().toLowerCase().replace(/(?:^|\s)\S/g, function (ch) {
            return ch.toUpperCase();
        });
    }

    function normalizeEmail(s) {
        return s.trim().toLowerCase();
    }

    // ── Regular field rules ──────────────────────────────────────────────
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
            id: 'Address',
            normalize: toTitleCase,
            validate: function (v) {
                const t = v.trim();
                if (!t) return 'Adress måste vara ifyllt';
                if (!/^[a-zA-ZåäöÅÄÖéèêëàâùûüîïôœæçÉÈÊËÀÂÙÛÜÎÏÔŒÆÇ][a-zA-ZåäöÅÄÖéèêëàâùûüîïôœæçÉÈÊËÀÂÙÛÜÎÏÔŒÆÇ\s]* \d+[a-zA-Z0-9]*$/.test(t))
                    return 'Ange gatunamn och husnummer med mellanslag, t.ex. Strandgatan 24';
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

    // ── Phone prefix enforcement ────────────────────────────────────────────
    const phoneInput = document.getElementById('PhoneNumber');
    if (phoneInput) {
        // Block backspace/delete into the locked prefix
        phoneInput.addEventListener('keydown', function (e) {
            if (!currentDialPrefix) return;
            const pos = this.selectionStart;
            const sel = this.selectionEnd;
            if (e.key === 'Backspace' && pos <= currentDialPrefix.length && pos === sel) {
                e.preventDefault();
            }
            if (e.key === 'Delete' && pos < currentDialPrefix.length) {
                e.preventDefault();
            }
            // Only allow digits, navigation keys and standard shortcuts after prefix
            const nav = ['ArrowLeft','ArrowRight','ArrowUp','ArrowDown','Tab','Home','End','Enter'];
            if (e.ctrlKey || e.metaKey || nav.includes(e.key)) return;
            if (e.key === 'Backspace' || e.key === 'Delete') return;
            if (!/^\d$/.test(e.key)) {
                e.preventDefault();
                return;
            }
            // Enforce max 8 digits
            const digits = this.value.slice(currentDialPrefix.length).replace(/\D/g, '');
            if (digits.length >= 8 && pos === sel) e.preventDefault();
        });

        // Sanitise on paste / autofill / any other input path
        phoneInput.addEventListener('input', function () {
            if (!currentDialPrefix) return;
            let val = this.value;
            if (!val.startsWith(currentDialPrefix)) val = currentDialPrefix;
            const digits = val.slice(currentDialPrefix.length).replace(/\D/g, '').slice(0, 8);
            const fixed  = currentDialPrefix + digits;
            if (fixed !== this.value) this.value = fixed;
        });
    }

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

    // ── Phone helpers (prefix-locked, digits only) ──────────────────────
    function normalizePhone(value) {
        if (!currentDialPrefix) return value.trim();
        const digits = value.slice(currentDialPrefix.length).replace(/\D/g, '').slice(0, 8);
        return currentDialPrefix + digits;
    }

    function validatePhone(value) {
        const trimmed = value.trim();
        if (!currentDialPrefix || !trimmed || trimmed === currentDialPrefix) {
            return 'Telefonnummer måste vara ifyllt';
        }
        const digits = trimmed.slice(currentDialPrefix.length);
        if (digits.length < 3) return 'Ange minst 3 siffror efter landkod';
        return null;
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
