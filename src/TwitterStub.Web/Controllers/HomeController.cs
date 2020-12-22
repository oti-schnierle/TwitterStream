using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Statistics;
using TwitterStub.Web.Models;

namespace TwitterStub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IStatisticsService _statisticsService;

        public HomeController(ILogger<HomeController> logger, IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _statisticsService.GetStatistics().ToListAsync();
            
            return View(values);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
