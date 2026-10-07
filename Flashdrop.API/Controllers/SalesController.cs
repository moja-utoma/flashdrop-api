using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Domain.Entities;
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
public class SalesController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{id}")]
    public Task<IActionResult> GetById(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateSaleRequest saleDto, CancellationToken token)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{id}")]
    public Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleRequest saleDto, CancellationToken token) 
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
