using Microsoft.AspNetCore.Mvc;

namespace NotificationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            Service = "NotificationService",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}