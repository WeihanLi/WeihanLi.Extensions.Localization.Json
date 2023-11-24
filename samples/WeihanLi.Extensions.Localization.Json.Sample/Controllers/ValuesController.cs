using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace WeihanLi.Extensions.Localization.Json.Sample.Controllers;

[Route("api/[controller]")]
public class ValuesController : Controller
{
    private readonly IStringLocalizer<ValuesController> _localizer;

    public ValuesController(IStringLocalizer<ValuesController> localizer)
    {
        _localizer = localizer;
    }

    // GET: api/values
    [HttpGet]
    public string Get() => _localizer["Culture"];
}
