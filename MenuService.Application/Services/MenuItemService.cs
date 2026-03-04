using FluentValidation;
using MenuService.Application.DTOs;
using MenuService.Application.Interfaces;
using MenuService.Domain.Entities;
using MenuService.Domain.Exceptions;

namespace MenuService.Application.Services
{
    public class MenuItemService
    {
        private readonly IMenuItemRepository _repository;
        private readonly IValidator<CreateMenuItemDto> _createValidator;
        private readonly IValidator<UpdateMenuItemDto> _updateValidator;

        public MenuItemService(
            IMenuItemRepository repository,
            IValidator<CreateMenuItemDto> createValidator,
            IValidator<UpdateMenuItemDto> updateValidator)
        {
            _repository = repository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        private static MenuItemResponseDto ToResponse(MenuItem item) => new(
            item.Id, item.Name, item.Description, item.Price,
            item.Category, item.Emoji, item.IsVegetarian,
            item.Badge, item.IsAvailable, item.CreatedAt, item.UpdatedAt
        );
        public async Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemDto dto, CancellationToken ct = default)
        {
            await _createValidator.ValidateAndThrowAsync(dto, ct);
            var menuItem = MenuItem.Create(dto.Name, dto.Description, dto.Price, dto.Category, dto.Emoji, dto.IsVegetarian, dto.Badge);
            await _repository.AddAsync(menuItem, ct);
            //return new MenuItemResponseDto(
            //    menuItem.Id,
            //    menuItem.Name,
            //    menuItem.Description,
            //    menuItem.Price,
            //    menuItem.Category,
            //    menuItem.Emoji,
            //    menuItem.IsVegetarian,
            //    menuItem.Badge,
            //    menuItem.IsAvailable,
            //    menuItem.CreatedAt,
            //    menuItem.UpdatedAt
            //);
            return ToResponse(menuItem);
        }
        public async Task<MenuItemResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var menuItem = await _repository.GetByIdAsync(id, ct)
                 ?? throw new MenuItemNotFoundException(id);
            return ToResponse(menuItem);
        }
        public async Task<IEnumerable<MenuItemResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var items = await _repository.GetAllAsync(ct);
            return items.Select(ToResponse);
        }
        public async Task<IEnumerable<MenuItemResponseDto>> GetByCategoryAsync(string category, CancellationToken ct = default)
        {
            var items = await _repository.GetByCategoryAsync(category, ct);
            return items.Select(ToResponse);
        }
        public async Task<IEnumerable<MenuItemResponseDto>> GetAvailableAsync(CancellationToken ct = default)
        {
            var items = await _repository.GetAvailableAsync(ct);
            return items.Select(ToResponse);
        }
        public async Task<MenuItemResponseDto> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto, CancellationToken ct = default)
        {
            await _updateValidator.ValidateAndThrowAsync(dto, ct);
            var menuItem = await _repository.GetByIdAsync(id, ct)
                ?? throw new MenuItemNotFoundException(id);
            menuItem.Update(dto.Name, dto.Description, dto.Price, dto.Category, dto.Emoji, dto.IsVegetarian, dto.Badge, dto.IsAvailable);
            await _repository.UpdateAsync(menuItem, ct);
            return ToResponse(menuItem);
        }
        public async Task DisableAsync(Guid id, CancellationToken ct = default)
        {
            var menuItem = await _repository.GetByIdAsync(id, ct)
                ?? throw new MenuItemNotFoundException(id);
            menuItem.Disable();
            await _repository.UpdateAsync(menuItem, ct);
        }
        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            if (!await _repository.ExistsAsync(id, ct))
                throw new MenuItemNotFoundException(id);
            await _repository.DeleteAsync(id, ct);
        }
    }
}
