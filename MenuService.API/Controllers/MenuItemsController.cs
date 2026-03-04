using MenuService.Application.DTOs;
using MenuService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly MenuItemService menuItemService;
        public MenuItemsController(MenuItemService menuItemService)
        {
            this.menuItemService = menuItemService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var menuItems = await menuItemService.GetAllAsync(ct);
            return Ok(menuItems);
        }
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailable(CancellationToken ct)
        {
            var menuItems = await menuItemService.GetAvailableAsync(ct);
            return Ok(menuItems);

        }
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(string category, CancellationToken ct)
        {
            var menuItems = await menuItemService.GetByCategoryAsync(category, ct);
            return Ok(menuItems);
        }
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var item = await menuItemService.GetByIdAsync(id, ct);
            return Ok(item);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto, CancellationToken ct)
        {
            var item = await menuItemService.CreateMenuItemAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemDto dto, CancellationToken ct)
        {
            var item = await menuItemService.UpdateMenuItemAsync(id, dto, ct);
            return Ok(item);
        }
        [HttpPatch("{id:guid}/disable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
        {
            await menuItemService.DisableAsync(id, ct);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await menuItemService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
