using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface ICalendarEventService
{
    Task<IEnumerable<CalendarEvent>> GetAllCalendarEventsAsync();
    Task CreateEventAsync(CalendarEvent calendarEvent);
    Task<CalendarEvent?> GetByIdAsync(string id);
    Task<IEnumerable<CalendarEvent>> GetByUserIdAsync(string userId);
    Task DeleteCalendarEventAsync(string id);
    Task UpdateAsync(CalendarEvent calendarEvent);
}
