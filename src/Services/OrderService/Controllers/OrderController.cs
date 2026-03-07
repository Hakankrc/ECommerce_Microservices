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
            
            _logger.LogInformation("OrderService GET isteği başarıyla karşılandı. Bağlantı tescillendi.");
            
            return Ok(new { message = "Order Service Ayakta ve Loglama Aktif!", time = DateTime.Now });
        }

        // Test için dummy bir sipariş ID'si dönen metod
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation("{OrderId} numaralı sipariş detayı istendi.", id);
            return Ok(new { OrderId = id, Status = "Hazırlanıyor" });
        }
    }
}