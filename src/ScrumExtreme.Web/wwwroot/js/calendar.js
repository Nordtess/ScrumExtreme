document.addEventListener('DOMContentLoaded', function () {

    var calendarEl = document.getElementById('calendar');
    if (!calendarEl) {
        console.error("calendar element saknas!");
        return;
    }

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',

        events: '/Calendar/GetEvents',

        eventClick: function (info) {
            alert(info.event.title);
        }
    });

    calendar.render();

   
});