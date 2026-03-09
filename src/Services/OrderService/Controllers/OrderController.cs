using Microsoft.AspNetCore.Mvc;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> _logger)
        {
            this._logger = _logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("OrderService GET request received. Connectivity verified.");
            return Ok(new { message = "Order Service is running and logging is active.", time = DateTime.Now });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation("Order details requested. OrderId: {OrderId}", id);
            return Ok(new { OrderId = id, Status = "InProgress" });
        }
    }
}