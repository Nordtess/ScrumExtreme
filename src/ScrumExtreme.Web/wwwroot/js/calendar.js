document.addEventListener('DOMContentLoaded', function () {

    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',

        events: '/Calendar/GetEvents', 

        eventClick: function (info) {
            alert(info.event.title);
        }
    });

    calendar.render();
});