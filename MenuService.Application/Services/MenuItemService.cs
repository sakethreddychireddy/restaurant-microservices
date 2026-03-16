using FluentValidation;
using MenuService.Application.DTOs;
using MenuService.Application.Interfaces;
using MenuService.Domain.Exceptions;

public class MenuItemService
{
    private readonly IMenuItemRepository _repository;
    private readonly IValidator<CreateMenuItemDto> _createValidator;
    private readonly IValidator<UpdateMenuItemDto> _updateValidator;
    private readonly IImageStorageService _imageStorage;   

    public MenuItemService(
        IMenuItemRepository repository,
        IValidator<CreateMenuItemDto> createValidator,
        IValidator<UpdateMenuItemDto> updateValidator,
        IImageStorageService imageStorage)                
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _imageStorage = imageStorage;
    }

    private static MenuItemResponseDto ToResponse(MenuItem item) => new(
        item.Id, item.Name, item.Description, item.Price,
        item.Category, item.ImageUrl,
        item.IsVegetarian, item.Badge,
        item.IsAvailable, item.CreatedAt, item.UpdatedAt
    );

    //public async Task<MenuItemResponseDto> CreateMenuItemAsync(
    //    CreateMenuItemDto dto, CancellationToken ct = default)
    //{
    //    await _createValidator.ValidateAndThrowAsync(dto, ct);
    //    var menuItem = MenuItem.Create(
    //        dto.Name, dto.Description, dto.Price,
    //        dto.Category, dto.ImageUrl,
    //        dto.IsVegetarian, dto.Badge);
    //    await _repository.AddAsync(menuItem, ct);
    //    return ToResponse(menuItem);
    //}

    public async Task<MenuItemResponseDto> CreateMenuItemAsync(
    CreateMenuItemDto dto, CancellationToken ct = default)
    {
        // resolve image
        string imageUrl;

        if (dto.ImageFile is not null)
        {
            if (dto.ImageFile.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException(
                    "Image size cannot exceed 5MB.");

            imageUrl = await _imageStorage.SaveImageAsync(
                Guid.NewGuid(), dto.ImageFile.OpenReadStream(),
                dto.ImageFile.ContentType, ct);
        }
        else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            imageUrl = dto.ImageUrl;
        }
        else
        {
            throw new InvalidOperationException(
                "Image is required. Please upload a file or provide a URL.");
        }

        var menuItem = MenuItem.Create(
            dto.Name, dto.Description, dto.Price,
            dto.Category, imageUrl,
            dto.IsVegetarian, dto.Badge);

        await _repository.AddAsync(menuItem, ct);
        return ToResponse(menuItem);
    }

    public async Task<MenuItemResponseDto> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var menuItem = await _repository.GetByIdAsync(id, ct)
            ?? throw new MenuItemNotFoundException(id);
        return ToResponse(menuItem);
    }

    public async Task<IEnumerable<MenuItemResponseDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items.Select(ToResponse);
    }

    public async Task<IEnumerable<MenuItemResponseDto>> GetByCategoryAsync(
        string category, CancellationToken ct = default)
    {
        var items = await _repository.GetByCategoryAsync(category, ct);
        return items.Select(ToResponse);
    }

    public async Task<IEnumerable<MenuItemResponseDto>> GetAvailableAsync(
        CancellationToken ct = default)
    {
        var items = await _repository.GetAvailableAsync(ct);
        return items.Select(ToResponse);
    }

    public async Task<MenuItemResponseDto> UpdateMenuItemAsync(
        Guid id, UpdateMenuItemDto dto, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, ct);
        var menuItem = await _repository.GetByIdAsync(id, ct)
            ?? throw new MenuItemNotFoundException(id);
        menuItem.Update(
            dto.Name, dto.Description, dto.Price,
            dto.Category, dto.ImageUrl,
            dto.IsVegetarian, dto.Badge, dto.IsAvailable);
        await _repository.UpdateAsync(menuItem, ct);
        return ToResponse(menuItem);
    }

    // ── Upload image file ──────────────────────────────────────
    public async Task<MenuItemResponseDto> UploadImageAsync(
        Guid id, Stream imageStream,
        string contentType, long fileSize,
        CancellationToken ct = default)
    {
        if (fileSize > 5 * 1024 * 1024)
            throw new InvalidOperationException(
                "Image size cannot exceed 5MB.");

        var menuItem = await _repository.GetByIdAsync(id, ct)
            ?? throw new MenuItemNotFoundException(id);

        // delete old image if exists
        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
            await _imageStorage.DeleteImageAsync(menuItem.ImageUrl, ct);

        // save new image
        var imageUrl = await _imageStorage.SaveImageAsync(
            id, imageStream, contentType, ct);

        menuItem.UpdateImage(imageUrl);
        await _repository.UpdateAsync(menuItem, ct);

        return ToResponse(menuItem);
    }

    // ── Update image via URL ───────────────────────────────────
    public async Task<MenuItemResponseDto> UpdateImageUrlAsync(
        Guid id, string imageUrl, CancellationToken ct = default)
    {
        var menuItem = await _repository.GetByIdAsync(id, ct)
            ?? throw new MenuItemNotFoundException(id);

        // delete old local image if exists
        if (!string.IsNullOrEmpty(menuItem.ImageUrl) &&
            menuItem.ImageUrl.StartsWith("/images/"))
            await _imageStorage.DeleteImageAsync(menuItem.ImageUrl, ct);

        menuItem.UpdateImage(imageUrl);
        await _repository.UpdateAsync(menuItem, ct);

        return ToResponse(menuItem);
    }

    public async Task DisableAsync(
        Guid id, CancellationToken ct = default)
    {
        var menuItem = await _repository.GetByIdAsync(id, ct)
            ?? throw new MenuItemNotFoundException(id);
        menuItem.Disable();
        await _repository.UpdateAsync(menuItem, ct);
    }

    public async Task DeleteAsync(
        Guid id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            throw new MenuItemNotFoundException(id);

        // delete image if exists
        var menuItem = await _repository.GetByIdAsync(id, ct);
        if (menuItem?.ImageUrl != null)
            await _imageStorage.DeleteImageAsync(menuItem.ImageUrl, ct);

        await _repository.DeleteAsync(id, ct);
    }
}