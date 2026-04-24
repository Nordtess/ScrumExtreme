using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class CalendarService : ICalendarEventService
{
    private readonly IRepository<CalendarEvent> _repository;

    public CalendarService(IRepository<CalendarEvent> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CalendarEvent>> GetAllCalendarEventsAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateEventAsync(CalendarEvent calendarEvent) =>
        await _repository.AddAsync(calendarEvent);

    public async Task<CalendarEvent?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<CalendarEvent>> GetByUserIdAsync(string userId)
    {
        var all = await _repository.GetAllAsync();
        return all.Where(o => o.UserId == userId);
    }

    public async Task DeleteCalendarEventAsync(string id) =>
        await _repository.DeleteAsync(id);

    
    public async Task UpdateAsync(CalendarEvent calendarEvent)
    {
        await _repository.UpdateAsync(calendarEvent.Id, calendarEvent);
    }
}