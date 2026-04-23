document.addEventListener('DOMContentLoaded', function () {

    // ── Data ─────────────────────────────────────────────────────────────
    let countries  = [];
    let labels     = {};   // { sv: { type, recipient, name, address, … }, … }

    // Swedish names for each language code (shown in the hint text)
    const langDisplayNames = {
        sv: 'Svenska',      no: 'Norska',         da: 'Danska',
        fi: 'Finska',       de: 'Tyska',          fr: 'Franska',
        es: 'Spanska',      pt: 'Portugisiska',   it: 'Italienska',
        nl: 'Nederl\u00e4ndska', pl: 'Polska',   ru: 'Ryska',
        ar: 'Arabiska',     zh: 'Kinesiska',      ja: 'Japanska',
        ko: 'Koreanska',    en: 'Engelska',
        th: 'Thailändska',  tr: 'Turkiska',       cs: 'Tjeckiska',
        sk: 'Slovakiska',   hu: 'Ungerska',       ro: 'Rumänska',
        bg: 'Bulgariska',   el: 'Grekiska',       uk: 'Ukrainska',
        hi: 'Hindi',        bn: 'Bengali',        ur: 'Urdu',
        fa: 'Persiska',     he: 'Hebreiska',      vi: 'Vietnamesiska',
        id: 'Indonesiska',  ms: 'Malajiska',      tl: 'Tagalog',
        sr: 'Serbiska',     hr: 'Kroatiska',      sl: 'Slovenska',
        lt: 'Litauiska',    lv: 'Lettiska',       et: 'Estniska',
        ca: 'Katalanska',   eu: 'Baskiska',       gl: 'Galiciska',
        is: 'Isländska',    ga: 'Iriska',         mt: 'Maltesiska',
        sq: 'Albanska',     mk: 'Makedonska',     bs: 'Bosniska',
        af: 'Afrikaans',    sw: 'Swahili'
    };

    // Fetch both JSON files in parallel
    Promise.all([
        fetch('/js/countries.json').then(function (r) { return r.json(); }),
        fetch('/js/shipping-labels.json').then(function (r) { return r.json(); })
    ]).then(function (results) {
        countries = results[0];
        labels    = results[1];

        // Auto-select language from server-provided country code (e.g. "SE", "US")
        var autoCode = (window.slCountryCode || '').toUpperCase();
        if (autoCode) {
            var match = countries.find(function (c) { return (c.iso || '').toUpperCase() === autoCode; });
            if (match) {
                searchInput.value = match.name;
                applyLabels(match.lang || 'en');
                return;
            }
        }
        // Fallback to Swedish labels
        applyLabels('sv');
    }).catch(function (e) {
        console.warn('[shippinglabel] Failed to load JSON files', e);
    });

    // ── Label swap ───────────────────────────────────────────────────────
    function applyLabels(lang) {
        const set = labels[lang] || labels['en'];
        const en  = labels['en'];

        // Swedish and English get single-language labels.
        // All other languages get "English / Local" bilingual labels.
        function lbl(enVal, localVal) {
            if (lang === 'sv' || lang === 'en') return localVal;
            return enVal + ' / ' + localVal;
        }

        document.getElementById('lbl-type').textContent            = lbl(en.type,       set.type);
        document.getElementById('lbl-recipient-title').textContent = lbl(en.recipient,  set.recipient);
        document.getElementById('lbl-name').textContent            = lbl(en.name,       set.name);
        document.getElementById('lbl-address').textContent         = lbl(en.address,    set.address);
        document.getElementById('lbl-postalCode').textContent      = lbl(en.postalCode, set.postalCode);
        document.getElementById('lbl-city').textContent            = lbl(en.city,       set.city);
        document.getElementById('lbl-country').textContent         = lbl(en.country,    set.country);
        document.getElementById('lbl-phone').textContent           = lbl(en.phone,      set.phone);

        const display = document.getElementById('slLangDisplay');
        if (display) display.textContent = langDisplayNames[lang] || lang;
    }

    // ── Country picker ───────────────────────────────────────────────────
    let currentMatches = [];
    let highlightedIdx = -1;

    const searchInput = document.getElementById('slCountrySearch');
    const dropdown    = document.getElementById('slCountryDropdown');

    searchInput.addEventListener('input', function () {
        const q = this.value.trim().toLowerCase();
        if (!q) { closeDropdown(); return; }

        const starts   = countries.filter(function (c) { return c.name.toLowerCase().startsWith(q); });
        const contains = countries.filter(function (c) { return !c.name.toLowerCase().startsWith(q) && c.name.toLowerCase().includes(q); });
        renderDropdown(starts.concat(contains).slice(0, 8));
    });

    searchInput.addEventListener('keydown', function (e) {
        if (dropdown.classList.contains('country-dropdown--hidden')) return;
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

    searchInput.addEventListener('blur', function () {
        setTimeout(closeDropdown, 160);
    });

    function renderDropdown(matches) {
        currentMatches = matches;
        highlightedIdx = -1;
        dropdown.innerHTML = '';
        matches.forEach(function (c) {
            const li = document.createElement('li');
            li.className  = 'country-option';
            li.textContent = c.name + '  ' + c.dial;
            li.addEventListener('mousedown', function (e) {
                e.preventDefault();
                selectCountry(c);
            });
            dropdown.appendChild(li);
        });
        dropdown.classList.remove('country-dropdown--hidden');
    }

    function updateHighlight() {
        dropdown.querySelectorAll('.country-option').forEach(function (li, i) {
            li.classList.toggle('country-option--active', i === highlightedIdx);
        });
    }

    function selectCountry(c) {
        searchInput.value = c.name;
        closeDropdown();
        applyLabels(c.lang || 'en');
    }

    function closeDropdown() {
        dropdown.classList.add('country-dropdown--hidden');
    }

    // ── Print ────────────────────────────────────────────────────────────
    const printBtn = document.getElementById('printBtn');
    if (printBtn) {
        printBtn.addEventListener('click', function () {
            window.print();
        });
    }
});
