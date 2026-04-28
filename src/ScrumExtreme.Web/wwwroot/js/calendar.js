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

        eventClick: function (info) {
            const orderId = info.event.extendedProps.orderId;
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

    // Modal open/close (admin only)
    const addEventBtn = document.getElementById("addEventBtn");
    const overlay = document.getElementById("calendarEventOverlay");
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

            const eventData = {
                userId:              document.getElementById("userId").value,
                orderId:             isOrder ? document.getElementById("orderId").value : null,
                eventType:           eventType,
                start:               isOrder ? null : toISODate(document.getElementById("start").value),
                end:                 isOrder ? null : toISODate(document.getElementById("end").value),
                orderStatusOverride: isOrder ? (document.getElementById("orderStatus").value || null) : null
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