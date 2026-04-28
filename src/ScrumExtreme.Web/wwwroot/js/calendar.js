let calendar;

document.addEventListener('DOMContentLoaded', function () {

    const calendarEl = document.getElementById('calendar');

    if (!calendarEl) {
        console.error("calendar element saknas!");
        return;
    }

    
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        height: '100%',
        events: '/Calendar/GetEvents',
        locale: 'sv-SE',

        eventClick: async function (info) {
            const confirmDelete = confirm(
                `Vill du ta bort eventet?\n${info.event.title}`
            );

            if (!confirmDelete) return;

            const response = await fetch('/Calendar/DeleteEvent', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(info.event.id)
            });

            const result = await response.json();

            if (result.success) {
                alert("Event borttaget!");
                calendar.refetchEvents();
            } else {
                alert("Kunde inte ta bort event");
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

            info.el.title = `${workerName}
            Order: ${orderNumber}
            Start: ${start}
            Slut: ${end}`;

        }
    });

    calendar.render();

    
    loadDropdowns();

    
    document.getElementById("addEventBtn").addEventListener("click", function () {
        const form = document.getElementById("eventForm");
        form.style.display = form.style.display === "none" ? "block" : "none";
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
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(eventData)
            });

            const result = await response.json();

            if (result.success) {
                alert("Event sparat!");

                
                calendar.refetchEvents();

                
                document.getElementById("eventForm").style.display = "none";
            } else {
                alert("Något gick fel");
            }
        } catch (error) {
            console.error("Fel vid sparande:", error);
        }
    });
});



async function loadDropdowns() {
    try {
        
        const userRes = await fetch('/Calendar/GetUsers');
        const users = await userRes.json();

        const userSelect = document.getElementById("userId");
        userSelect.innerHTML = "";

        users.forEach(u => {
            const option = document.createElement("option");
            option.value = u.id;
            option.text = u.name;
            userSelect.appendChild(option);
        });

        
        const orderRes = await fetch('/Calendar/GetOrders');
        const orders = await orderRes.json();

        const orderSelect = document.getElementById("orderId");
        orderSelect.innerHTML = "";

        orders.forEach(o => {
            const option = document.createElement("option");
            option.value = o.id;
            option.text = o.name;
            orderSelect.appendChild(option);
        });

    } catch (error) {
        console.error("Fel vid laddning av dropdowns:", error);
    }
}