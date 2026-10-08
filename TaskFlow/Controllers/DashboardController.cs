using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet]
        public IActionResult GetSummary()
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var summary = _dashboardService.GetSummary(userId, role);
            return Ok(summary);
        }
    }
}
