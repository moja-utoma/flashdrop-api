using Microsoft.AspNetCore.Mvc;

namespace Flashdrop.API.Extensions;

public static class ControllerExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, T? value)
        where T : class
    {
        return value is null ? controller.NotFound() : controller.Ok(value);
    }
}
