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

            const orderNumber = info.event.extendedProps.orderNumber;
            const workerName = info.event.extendedProps.workerName;

            info.el.title = `${workerName}\nOrder: ${orderNumber}\nStart: ${start}\nSlut: ${end}`;
            info.el.style.cursor = 'pointer';
        }
    });

    calendar.render();

    // Modal open/close (admin only)
    const addEventBtn = document.getElementById("addEventBtn");
    const overlay = document.getElementById("calendarEventOverlay");
    const cancelBtn = document.getElementById("cancelEvent");

    if (addEventBtn && overlay) {
        loadDropdowns();

        addEventBtn.addEventListener("click", function () {
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
            const eventData = {
                userId: document.getElementById("userId").value,
                orderId: document.getElementById("orderId").value,
                start: document.getElementById("start").value,
                end: document.getElementById("end").value
            };

            try {
                const response = await fetch('/Calendar/CreateEvent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(eventData)
                });

                const result = await response.json();

                if (result.success) {
                    overlay.style.display = "none";
                    calendar.refetchEvents();
                } else {
                    alert("Något gick fel");
                }
            } catch (error) {
                console.error("Fel vid sparande:", error);
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