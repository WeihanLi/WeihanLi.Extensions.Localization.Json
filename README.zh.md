# WeihanLi.Extensions.Localization.Json [![WeihanLi.Extensions.Localization.Json](https://img.shields.io/nuget/v/WeihanLi.Extensions.Localization.Json.svg)](https://www.nuget.org/packages/WeihanLi.Extensions.Localization.Json/)

## Intro

dotnet 基于 json 的本地化组件，支持基于 culture 的资源路径

## Build

[![AzureDevOps Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.WeihanLi.Extensions.Localization.Json?branchName=dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=25&branchName=dev)

[![Github Build Status](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/workflows/dotnet-ci/badge.svg?branch=dev)](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/actions?query=workflow%3Adotnet-ci+branch%3Adev)

## GetStarted

注册服务：

``` csharp
services.AddJsonLocalization(options =>
    {
        options.ResourcesPath = Configuration.GetAppSetting("ResourcesPath");
        options.ResourcesPathType = ResourcesPathType.TypeBased; // 默认方式和微软找资源的方式类似
        // options.ResourcesPathType = ResourcesPathType.CultureBased; // 在对应的 culture 子目录下寻找资源文件，可以参考后面的示例
    });
```

中间件配置(如果是asp.net core，和之前一样):

``` csharp
app.UseRequestLocalization();
```

That's it~

## 添加你的资源文件

### TypeBased 资源文件的路径

**For Types:**

`Home/Index` => Controllers/HomeController

资源路径：

- [ResourcesPath]/Controllers/HomeController.[cultureName].json

示例:

- Resources/Controllers/HomeController.en.json
- Resources/Controllers/HomeController.zh.json

**For Razor 视图:**

示例:

- Resources/Views/Home/Index.en.json
- Resources/Views/Home/Index.zh.json

### CultureBased 资源文件路径

**For Types:**

`Home/Index` => Controllers/HomeController

资源路径:

- [ResourcesPath]/[cultureName]/Controllers/HomeController.json

示例：

- Resources/en/Controllers/HomeController.json
- Resources/zh/Controllers/HomeController.json

**For Razor 视图:**

示例：

- Resources/en/Views/Home/Index.json
- Resources/zh/Views/Home/Index.json

**Copy your resource files to output:**

需要设置将资源文件拷贝到输出目录，否则会找不到资源文件，可以在启动项目项目文件中加入以下示例代码：

``` xml
<ItemGroup>
<Content Update="Resources\**\*.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>
```

上面的配置会将 `Resources` 目录下的所有 json 文件拷贝到输出目录下，可以根据自己的需要进行修改

## Use

用法和之前是一样的

Controller 示例：

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

Razor 视图示例：

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

资源文件示例：

``` json
{
  "Culture": "中文"
}
```

## Samples

- [AspNetCoreSample](https://github.com/WeihanLi/WeihanLi.Extensions.Localization.Json/tree/dev/samples/WeihanLi.Extensions.Localization.Json.Sample)
- [Reservation](https://github.com/OpenReservation/ReservationServer)
- [DbTool](https://github.com/WeihanLi/DbTool)

## Contact

Contact me via <weihanli@outlook.com> if you need
