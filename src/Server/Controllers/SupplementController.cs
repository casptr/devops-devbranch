
using Foodtruck.Shared.Supplements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
public class SupplementController : ControllerBase
{
    private readonly ISupplementService supplementService;

    public SupplementController(ISupplementService supplementService)
    {
        this.supplementService = supplementService;
    }


    [SwaggerOperation("Returns all supplements based on filters")]
    [HttpGet, AllowAnonymous]
    public async Task<SupplementResult.Index> GetIndex([FromQuery] SupplementRequest.Index request)
    {
        return await supplementService.GetIndexAsync(request);
    }

    [SwaggerOperation("Returns all supplements")]
    [HttpGet("all"), AllowAnonymous]
    public async Task<SupplementResult.Index> GetAll()
    {
        return await supplementService.GetAllAsync();
    }

    [SwaggerOperation("Returns a specific supplement.")]
    [HttpGet("{supplementId}"), AllowAnonymous]
    public async Task<SupplementDto.Detail> GetDetail(int supplementId)
    {
        return await supplementService.GetDetailAsync(supplementId);
    }

    [SwaggerOperation("Creates a new supplement.")]
    [HttpPost] // TODO: Roles - Authorize(Roles = Roles.Administrator)
    public async Task<IActionResult> Create(SupplementDto.Mutate model)
    {
        var supplementId = await supplementService.CreateAsync(model);
        return CreatedAtAction(nameof(Create), new { id = supplementId });
    }

    [SwaggerOperation("Edites an existing supplement.")]
    [HttpPut("{supplementId}")] // TODO: Roles - Authorize(Roles = Roles.Administrator)
    public async Task<IActionResult> Edit(int supplementId, SupplementDto.Mutate model)
    {
        var result = await supplementService.EditAsync(supplementId, model);
        return AcceptedAtAction(nameof(Edit), result);
    }

    [SwaggerOperation("Deletes an existing supplement in the catalog.")]
    [HttpDelete("{supplementId}")] // TODO: Roles - Authorize(Roles = Roles.Administrator)
    public async Task<IActionResult> Delete(int supplementId)
    {
        await supplementService.DeleteAsync(supplementId);
        return NoContent();
    }

    //[SwaggerOperation("Add image to existing supplement.")]
    //[HttpPost("{supplementId}/addimage")]
    //public async Task<IActionResult> AddImage(int supplementId)
    //{
    //    await supplementService.AddImage(supplementId);
    //    return CreatedAtAction(nameof(Create), supplementId);
    //}

}
