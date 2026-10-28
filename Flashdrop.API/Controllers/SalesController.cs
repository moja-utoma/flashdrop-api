using Flashdrop.Application.Sales;
using Flashdrop.Application.Sales.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Flashdrop.API.Controllers;

// TODO: Admin-only create/update/cancel, public read (with status filter: live/upcoming/ended), for Sale.
/* TODO: Acceptance criteria:
         Creating a sale validates StartsAt < EndsAt and TotalStock > 0,
         AvailableStock initializes to TotalStock */

/// <summary>
/// Manages sales operations including admin-only create/update/cancel,
/// public read.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SalesController(ISalesService salesService) : ControllerBase
{
    private readonly ISalesService _salesService = salesService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var sales = await _salesService.GetAllAsync(token);
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var sale = await _salesService.GetByIdAsync(id, token);
        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest saleDto, CancellationToken token)
    {
        var saleId = await _salesService.CreateAsync(saleDto, token);
        return Ok(saleId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleRequest saleDto, CancellationToken token)
    {
        await _salesService.UpdateAsync(id, saleDto, token);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        await _salesService.DeleteAsync(id, token);
        return NoContent();
    }
}