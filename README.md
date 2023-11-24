# WeihanLi.Extensions.Localization.Json [![WeihanLi.Extensions.Localization.Json](https://img.shields.io/nuget/v/WeihanLi.Extensions.Localization.Json.svg)](https://www.nuget.org/packages/WeihanLi.Extensions.Localization.Json/)

## Intro

dotnet JSON file based localization

## Build

[![AzureDevOps Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.WeihanLi.Extensions.Localization.Json?branchName=dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=25&branchName=dev)

[![Github Build Status](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/workflows/dotnet-ci/badge.svg?branch=dev)](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/actions?query=workflow%3Adotnet-ci+branch%3Adev)

## GetStarted

register required services:

``` csharp
services.AddJsonLocalization(options =>
    {
        options.ResourcesPath = Configuration.GetAppSetting("ResourcesPath");
        options.ResourcesPathType = ResourcesPathType.TypeBased; // by default, looking for resourceFile like Microsoft do
        // options.ResourcesPathType = ResourcesPathType.CultureBased; // looking for resource file in culture sub dir see details follows
    });
```

middleware config(the same with before):

``` csharp
app.UseRequestLocalization();
```

That's it~

## Add your resource files

### TypeBasedResourcePath

**For Types:**

`Home/Index` => Controllers/HomeController

the resource path looking for:

- Controllers/HomeController.[cultureName].json

for example:

- Resources/Controllers/HomeController.en.json
- Resources/Controllers/HomeController.zh.json

**For RazorViews:**

for example:

- Resources/Views/Home/Index.en.json
- Resources/Views/Home/Index.zh.json

### CultureBasedResourcePath

**For Types:**

`Home/Index` => Controllers/HomeController

the resource path looking for:

- Resources/[cultureName]/Controllers/HomeController.json

for example:

- Resources/en/Controllers/HomeController.json
- Resources/zh/Controllers/HomeController.json

**For RazorViews:**

for example:

- Resources/en/Views/Home/Index.json
- Resources/zh/Views/Home/Index.json

**Copy your resource files to output:**

you had to set resource files copy to output to make it works normal

add the follows sample config to your startup project file:

``` xml
<ItemGroup>
<Content Update="Resources\**\*.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>
```

the config above is made to make sure your json resource files in `Resources` dir copied to output, change it if you need

## Use

just like what you do before:

Controller sample:

``` csharp
public class ValuesController : Controller
{
    private readonly IStringLocalizer<ValuesController> _localizer;

    public ValuesController(IStringLocalizer<ValuesController> localizer)
    {
        _localizer = localizer;
    }

    // GET: api/<controller>
    [HttpGet]
    public string Get()
    {
        return _localizer["Culture"];
    }
}
```

Razor View Sample:

``` razor
@using Microsoft.AspNetCore.Mvc.Localization
@using Microsoft.Extensions.Localization
@using WeihanLi.Extensions.Localization.Json.Sample.Controllers
@inject IHtmlLocalizer<HomeController> HtmlLocalizer
@inject IStringLocalizer<HomeController> StringLocalizer
@inject IViewLocalizer ViewLocalizer
@{
    ViewData["Title"] = "Index";
}

<h2>Index</h2>

<div>string: @StringLocalizer["Hello"]</div>

<div>html: @HtmlLocalizer["Hello"]</div>

<div>view: @ViewLocalizer["Hello"]</div>
```

Resource file sample:

``` json
{
  "Culture": "English"
}
```

## Samples

- [AspNetCoreSample](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/tree/dev/samples/WeihanLi.Extensions.Localization.Json.Sample)
- [Reservation](https://github.com/OpenReservation/ReservationServer)
- [DbTool](https://github.com/WeihanLi/DbTool)

## Contact

Contact me via <weihanli@outlook.com> if you need
