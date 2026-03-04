using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly Application.Services.OrderService _service;

        public OrdersController(Application.Services.OrderService service)
            => _service = service;

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderDto dto, CancellationToken ct)
        {
            var order = await _service.CreateAsync(dto, CurrentUserId, ct);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var order = await _service.GetByIdAsync(id, ct);
            return Ok(order);
        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders(CancellationToken ct)
        {
            var orders = await _service.GetMyOrdersAsync(CurrentUserId, ct);
            return Ok(orders);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var orders = await _service.GetAllAsync(ct);
            return Ok(orders);
        }
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(
            Guid id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
        {
            var order = await _service.UpdateStatusAsync(id, dto, ct);
            return Ok(order);
        }
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            await _service.CancelAsync(id, ct);
            return NoContent();
        }
    }
}
