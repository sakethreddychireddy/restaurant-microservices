using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController(ProfileUseCase profileUseCase) : ControllerBase
    {
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ── Profile ────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var profile = await profileUseCase
                .GetProfileAsync(CurrentUserId, ct);
            return profile is null ? NotFound() : Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request,
            CancellationToken ct)
        {
            var profile = await profileUseCase
                .UpdateProfileAsync(CurrentUserId, request, ct);
            return profile is null ? NotFound() : Ok(profile);
        }

        // ── Addresses ──────────────────────────────────────────
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses(CancellationToken ct)
        {
            var addresses = await profileUseCase
                .GetAddressesAsync(CurrentUserId, ct);
            return Ok(addresses);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(
            [FromBody] AddressRequest request,
            CancellationToken ct)
        {
            var address = await profileUseCase
                .AddAddressAsync(CurrentUserId, request, ct);
            return Ok(address);
        }

        [HttpPut("addresses/{addressId:guid}")]
        public async Task<IActionResult> UpdateAddress(
            Guid addressId,
            [FromBody] AddressRequest request,
            CancellationToken ct)
        {
            var address = await profileUseCase
                .UpdateAddressAsync(CurrentUserId, addressId, request, ct);
            return address is null ? NotFound() : Ok(address);
        }

        [HttpDelete("addresses/{addressId:guid}")]
        public async Task<IActionResult> DeleteAddress(
            Guid addressId,
            CancellationToken ct)
        {
            var result = await profileUseCase
                .DeleteAddressAsync(CurrentUserId, addressId, ct);
            return result ? NoContent() : NotFound();
        }
    }
}