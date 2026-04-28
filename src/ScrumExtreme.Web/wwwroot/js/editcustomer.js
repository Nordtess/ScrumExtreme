(() => {
    /* ── DOM refs ──────────────────────────────────────────────── */
    const overlay         = document.getElementById('editCustomerOverlay');
    const openBtn         = document.getElementById('openEditCustomerBtn');
    const closeBtn        = document.getElementById('closeEditCustomerBtn');
    const cancelBtn       = document.getElementById('cancelEditCustomerBtn');
    const confirmBtn      = document.getElementById('confirmEditCustomerBtn');
    const submitError     = document.getElementById('ec-submitError');

    const fields = {
        FirstName:   document.getElementById('ec-FirstName'),
        LastName:    document.getElementById('ec-LastName'),
        Email:       document.getElementById('ec-Email'),
        Address:     document.getElementById('ec-Address'),
        PostalCode:  document.getElementById('ec-PostalCode'),
        City:        document.getElementById('ec-City'),
        PhoneNumber: document.getElementById('ec-PhoneNumber'),
    };

    const countrySearch   = document.getElementById('ec-countrySearch');
    const countryDropdown = document.getElementById('ec-countryDropdown');
    const countryHidden   = document.getElementById('ec-Country');
    const countryCodeHidden = document.getElementById('ec-CountryCode');
    const phoneField      = fields.PhoneNumber;

    /* ── Country picker ────────────────────────────────────────── */
    let allCountries = [];

    fetch('/js/countries.json')
        .then(r => r.json())
        .then(data => { allCountries = data; });

    function renderDropdown(list) {
        countryDropdown.innerHTML = '';
        if (list.length === 0) {
            countryDropdown.classList.add('country-dropdown--hidden');
            return;
        }
        list.forEach(c => {
            const li = document.createElement('li');
            li.textContent = `${c.flag ?? ''} ${c.name}`;
            li.addEventListener('mousedown', () => selectCountry(c));
            countryDropdown.appendChild(li);
        });
        countryDropdown.classList.remove('country-dropdown--hidden');
    }

    function selectCountry(c) {
        countrySearch.value      = c.name;
        countryHidden.value      = c.name;
        countryCodeHidden.value  = c.iso ?? '';
        countryDropdown.classList.add('country-dropdown--hidden');
        clearError('Country');

        // Update phone prefix
        const prefix = c.dial ?? '';
        phoneField.value = prefix;
    }

    countrySearch.addEventListener('input', () => {
        const q = countrySearch.value.trim().toLowerCase();
        countryHidden.value = '';
        if (!q) { countryDropdown.classList.add('country-dropdown--hidden'); return; }
        renderDropdown(allCountries.filter(c => c.name.toLowerCase().includes(q)));
    });

    countrySearch.addEventListener('blur', () => {
        setTimeout(() => countryDropdown.classList.add('country-dropdown--hidden'), 150);
    });

    countrySearch.addEventListener('focus', () => {
        const q = countrySearch.value.trim().toLowerCase();
        if (q) renderDropdown(allCountries.filter(c => c.name.toLowerCase().includes(q)));
    });

    /* ── Validation ────────────────────────────────────────────── */
    function showError(field, msg) {
        const el = document.getElementById(`ec-error-${field}`);
        if (el) el.textContent = msg;
    }

    function clearError(field) {
        const el = document.getElementById(`ec-error-${field}`);
        if (el) el.textContent = '';
    }

    function clearAllErrors() {
        ['FirstName','LastName','Email','Address','PostalCode','City','Country','PhoneNumber']
            .forEach(f => clearError(f));
        if (submitError) submitError.textContent = '';
    }

    const emailRe    = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const postalRe   = /^\d{3}\s?\d{2}$/;
    const phoneRe    = /^\+?\d[\d\s\-()]{6,19}$/;

    function validate() {
        let ok = true;

        if (!fields.FirstName.value.trim()) {
            showError('FirstName', 'Förnamn krävs.'); ok = false;
        }
        if (!fields.LastName.value.trim()) {
            showError('LastName', 'Efternamn krävs.'); ok = false;
        }
        if (!emailRe.test(fields.Email.value.trim())) {
            showError('Email', 'Ange en giltig e-postadress.'); ok = false;
        }
        if (!fields.Address.value.trim()) {
            showError('Address', 'Adress krävs.'); ok = false;
        }
        if (!postalRe.test(fields.PostalCode.value.trim())) {
            showError('PostalCode', 'Ange ett giltigt postnummer (t.ex. 703 44).'); ok = false;
        }
        if (!fields.City.value.trim()) {
            showError('City', 'Stad krävs.'); ok = false;
        }
        if (!countryHidden.value) {
            showError('Country', 'Välj ett land från listan.'); ok = false;
        }
        if (!phoneRe.test(fields.PhoneNumber.value.trim())) {
            showError('PhoneNumber', 'Ange ett giltigt telefonnummer.'); ok = false;
        }

        return ok;
    }

    /* ── Modal open / close ────────────────────────────────────── */
    function openModal() {
        clearAllErrors();
        overlay.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }

    function closeModal() {
        overlay.style.display = 'none';
        document.body.style.overflow = '';
    }

    openBtn.addEventListener('click', openModal);
    closeBtn.addEventListener('click', closeModal);
    cancelBtn.addEventListener('click', closeModal);
    overlay.addEventListener('click', e => { if (e.target === overlay) closeModal(); });

    /* ── Inline error clear on input ───────────────────────────── */
    Object.entries(fields).forEach(([name, el]) => {
        el.addEventListener('input', () => clearError(name));
    });

    /* ── Submit ────────────────────────────────────────────────── */
    confirmBtn.addEventListener('click', async () => {
        clearAllErrors();
        if (!validate()) return;

        const customerId = confirmBtn.dataset.customerId
            || window.location.pathname.split('/').pop();

        const payload = {
            firstName:   fields.FirstName.value.trim(),
            lastName:    fields.LastName.value.trim(),
            email:       fields.Email.value.trim().toLowerCase(),
            address:     fields.Address.value.trim(),
            postalCode:  fields.PostalCode.value.trim(),
            city:        fields.City.value.trim(),
            country:     countryHidden.value,
            countryCode: countryCodeHidden.value,
            phoneNumber: fields.PhoneNumber.value.trim(),
        };

        try {
            confirmBtn.disabled = true;
            const res = await fetch(`/AddCustomer/UpdateCustomer/${customerId}`, {
                method:  'POST',
                headers: { 'Content-Type': 'application/json' },
                body:    JSON.stringify(payload),
            });

            if (res.ok) {
                window.location.reload();
            } else {
                if (submitError) submitError.textContent = 'Något gick fel. Försök igen.';
            }
        } catch {
            if (submitError) submitError.textContent = 'Nätverksfel. Kontrollera anslutningen.';
        } finally {
            confirmBtn.disabled = false;
        }
    });
})();
