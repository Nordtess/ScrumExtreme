using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string period = "month")
    {
        if (period != "week" && period != "month" && period != "quarter" && period != "year")
            period = "month";

        var result = await _statisticsService.GetStatisticsAsync(period);
        return View(result);
    }
}
