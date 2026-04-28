let calendar;

document.addEventListener('DOMContentLoaded', function () {

    const calendarEl = document.getElementById('calendar');

    if (!calendarEl) {
        console.error("calendar element saknas!");
        return;
    }

    const isAdmin = (window.calendarUserRole === 'admin');

    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        height: '100%',
        events: '/Calendar/GetEvents',
        locale: 'sv-SE',
        displayEventTime: false,

        eventClick: function (info) {
            const orderId = info.event.extendedProps.orderId;

            if (isAdmin) {
                // Admin: show delete confirmation for non-order events;
                // for order events navigate to order details on left-click but
                // show delete if Ctrl/Meta is held
                if (!orderId || info.jsEvent.ctrlKey || info.jsEvent.metaKey) {
                    const deleteOverlay = document.getElementById("deleteEventOverlay");
                    const deleteDesc    = document.getElementById("deleteEventDescription");
                    if (deleteOverlay && deleteDesc) {
                        deleteDesc.textContent = info.event.title;
                        deleteOverlay._eventId = info.event.id;
                        deleteOverlay.style.display = "flex";
                    }
                    return;
                }
            }

            if (orderId) {
                window.location.href = '/Orders/Details/' + orderId;
            }
        },

        eventDidMount: function (info) {
            const start = info.event.start
                ? info.event.start.toLocaleString('sv-SE', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                })
                : '';

            const end = info.event.end
                ? info.event.end.toLocaleString('sv-SE', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                })
                : '';

            const orderId    = info.event.extendedProps.orderId;
            const orderNumber = info.event.extendedProps.orderNumber;
            const workerName = info.event.extendedProps.workerName;

            if (orderId) {
                info.el.title = `${workerName}\nOrder: ${orderNumber}\nStart: ${start}\nSlut: ${end}`;
                info.el.style.cursor = 'pointer';
            } else {
                info.el.title = `${workerName}\nStart: ${start}\nSlut: ${end}`;
            }
        }
    });

    calendar.render();

    // Delete event modal
    const deleteOverlay     = document.getElementById("deleteEventOverlay");
    const cancelDeleteBtn   = document.getElementById("cancelDeleteEvent");
    const confirmDeleteBtn  = document.getElementById("confirmDeleteEvent");

    if (deleteOverlay) {
        cancelDeleteBtn.addEventListener("click", function () {
            deleteOverlay.style.display = "none";
        });

        deleteOverlay.addEventListener("click", function (e) {
            if (e.target === deleteOverlay) deleteOverlay.style.display = "none";
        });

        confirmDeleteBtn.addEventListener("click", async function () {
            const eventId = deleteOverlay._eventId;
            if (!eventId) return;

            try {
                const response = await fetch('/Calendar/DeleteEvent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(eventId)
                });

                if (response.ok) {
                    deleteOverlay.style.display = "none";
                    calendar.refetchEvents();
                } else {
                    alert("Något gick fel vid borttagning.");
                }
            } catch (err) {
                console.error("Fel vid borttagning:", err);
                alert("Något gick fel vid borttagning.");
            }
        });
    }

    // ── Redigera event modal ──────────────────────────────────────────────
    const editEventBtn    = document.getElementById("editEventBtn");
    const editOverlay     = document.getElementById("editEventOverlay");
    const cancelEditBtn   = document.getElementById("cancelEditEvent");
    const deleteEditBtn   = document.getElementById("deleteEditEvent");
    const saveEditBtn     = document.getElementById("saveEditEvent");
    const editTypeFilter  = document.getElementById("editTypeFilter");
    const editEventSelect = document.getElementById("editEventId");
    const editDateFields  = document.getElementById("editDateFields");
    const editOrderFields = document.getElementById("editOrderFields");
    let editEventsCache   = [];

    function toISODateEdit(dateStr) {
        return dateStr ? dateStr + "T00:00:00" : null;
    }

    function populateEditFields(eventId) {
        const ev = editEventsCache.find(e => e.id === eventId);
        if (!ev) return;
        const isOrder = !!ev.orderId;
        editDateFields.style.display  = isOrder ? "none" : "";
        editOrderFields.style.display = isOrder ? "" : "none";
        if (isOrder) {
            document.getElementById("editOrderDate").value   = ev.start || "";
            document.getElementById("editOrderStatus").value = "";
        } else {
            document.getElementById("editStart").value = ev.start || "";
            document.getElementById("editEnd").value   = ev.end   || "";
        }
    }

    function rebuildEditEventDropdown(type) {
        const filtered = editEventsCache.filter(e => e.eventType === type);
        editEventSelect.innerHTML = "";
        filtered.forEach(e => {
            const opt = document.createElement("option");
            opt.value = e.id;
            opt.text  = e.title;
            editEventSelect.appendChild(opt);
        });
        if (filtered.length > 0) {
            populateEditFields(filtered[0].id);
        } else {
            editDateFields.style.display  = "none";
            editOrderFields.style.display = "none";
        }
    }

    async function loadEditEvents() {
        try {
            const res = await fetch('/Calendar/GetEventsForEdit');
            editEventsCache = await res.json();
            // Default to the first type that has events
            editTypeFilter.value = "order";
            rebuildEditEventDropdown("order");
        } catch (err) {
            console.error("Fel vid laddning av events:", err);
        }
    }

    if (editEventBtn && editOverlay) {
        editEventBtn.addEventListener("click", async function () {
            await loadEditEvents();
            editOverlay.style.display = "flex";
        });

        cancelEditBtn.addEventListener("click", function () {
            editOverlay.style.display = "none";
        });

        editOverlay.addEventListener("click", function (e) {
            if (e.target === editOverlay) editOverlay.style.display = "none";
        });

        editTypeFilter.addEventListener("change", function () {
            rebuildEditEventDropdown(this.value);
        });

        editEventSelect.addEventListener("change", function () {
            populateEditFields(this.value);
        });

        deleteEditBtn.addEventListener("click", async function () {
            const eventId = editEventSelect.value;
            if (!eventId) return;
            if (!confirm("Är du säker på att du vill ta bort eventet?")) return;
            try {
                const response = await fetch('/Calendar/DeleteEvent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(eventId)
                });
                if (response.ok) {
                    editOverlay.style.display = "none";
                    calendar.refetchEvents();
                } else {
                    alert("Något gick fel vid borttagning.");
                }
            } catch (err) {
                console.error("Fel vid borttagning:", err);
                alert("Något gick fel vid borttagning.");
            }
        });

        saveEditBtn.addEventListener("click", async function () {
            const eventId = editEventSelect.value;
            const ev      = editEventsCache.find(e => e.id === eventId);
            if (!ev) return;
            const isOrder = !!ev.orderId;
            const data = {
                id:                  eventId,
                start:               isOrder ? toISODateEdit(document.getElementById("editOrderDate").value)
                                             : toISODateEdit(document.getElementById("editStart").value),
                end:                 isOrder ? toISODateEdit(document.getElementById("editOrderDate").value)
                                             : toISODateEdit(document.getElementById("editEnd").value),
                orderStatusOverride: isOrder ? (document.getElementById("editOrderStatus").value || null) : null
            };
            try {
                const response = await fetch('/Calendar/EditEvent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(data)
                });
                if (response.ok) {
                    const result = await response.json();
                    if (result.success) {
                        editOverlay.style.display = "none";
                        calendar.refetchEvents();
                    } else {
                        alert("Något gick fel vid sparande.");
                    }
                } else {
                    alert("Serverfel vid sparande.");
                }
            } catch (err) {
                console.error("Fel vid sparande:", err);
                alert("Något gick fel vid sparande.");
            }
        });
    }
    // ─────────────────────────────────────────────────────────────────────

    // Modal open/close (admin only)
    const addEventBtn = document.getElementById("addEventBtn");    const overlay = document.getElementById("calendarEventOverlay");
    const cancelBtn = document.getElementById("cancelEvent");

    if (addEventBtn && overlay) {
        loadDropdowns();

        // Toggle order vs date fields based on event type
        const eventTypeSelect = document.getElementById("eventType");
        const orderFields = document.getElementById("orderFields");
        const dateFields  = document.getElementById("dateFields");

        function updateFieldVisibility() {
            const isOrder = eventTypeSelect.value === "order";
            orderFields.style.display = isOrder ? "" : "none";
            dateFields.style.display  = isOrder ? "none" : "";
        }
        eventTypeSelect.addEventListener("change", updateFieldVisibility);

        // When start date is picked, constrain end date to be >= start
        document.getElementById("start").addEventListener("change", function () {
            const endInput = document.getElementById("end");
            endInput.min = this.value;
            if (endInput.value && endInput.value < this.value) {
                endInput.value = this.value;
            }
        });

        addEventBtn.addEventListener("click", function () {
            updateFieldVisibility();
            overlay.style.display = "flex";
        });

        if (cancelBtn) {
            cancelBtn.addEventListener("click", function () {
                overlay.style.display = "none";
            });
        }

        overlay.addEventListener("click", function (e) {
            if (e.target === overlay) overlay.style.display = "none";
        });

        document.getElementById("saveEvent").addEventListener("click", async function () {
            const eventType = eventTypeSelect.value;
            const isOrder   = eventType === "order";

            function toISODate(dateStr) {
                return dateStr ? dateStr + "T00:00:00" : null;
            }

            const estimatedDate = isOrder ? document.getElementById("orderEstimatedDate").value : null;

            const eventData = {
                userId:              document.getElementById("userId").value,
                orderId:             isOrder ? document.getElementById("orderId").value : null,
                eventType:           eventType,
                start:               isOrder ? toISODate(estimatedDate) : toISODate(document.getElementById("start").value),
                end:                 isOrder ? toISODate(estimatedDate) : toISODate(document.getElementById("end").value),
                orderStatusOverride: null
            };

            try {
                const response = await fetch('/Calendar/CreateEvent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(eventData)
                });

                if (!response.ok) {
                    const text = await response.text();
                    console.error("Server error:", response.status, text);
                    alert(`Serverfel ${response.status}. Kontrollera konsolen för detaljer.`);
                    return;
                }

                const result = await response.json();

                if (result.success) {
                    overlay.style.display = "none";
                    calendar.refetchEvents();
                } else {
                    alert("Något gick fel");
                }
            } catch (error) {
                console.error("Fel vid sparande:", error);
                alert("Något gick fel vid sparande. Försök igen.");
            }
        });
    }
});


async function loadDropdowns() {
    try {
        const userRes = await fetch('/Calendar/GetUsers');
        const users = await userRes.json();

        const userSelect = document.getElementById("userId");
        if (userSelect) {
            userSelect.innerHTML = "";
            users.forEach(u => {
                const option = document.createElement("option");
                option.value = u.id;
                option.text = u.name;
                userSelect.appendChild(option);
            });
        }

        const orderRes = await fetch('/Calendar/GetOrders');
        const orders = await orderRes.json();

        const orderSelect = document.getElementById("orderId");
        if (orderSelect) {
            orderSelect.innerHTML = "";
            orders.forEach(o => {
                const option = document.createElement("option");
                option.value = o.id;
                option.text = o.name;
                orderSelect.appendChild(option);
            });
        }
    } catch (error) {
        console.error("Fel vid laddning av dropdowns:", error);
    }
}