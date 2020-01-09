using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WeihanLi.Extensions.Localization.Json
{
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache =
             new ConcurrentDictionary<string, JsonStringLocalizer>();

        private readonly string _resourcesRelativePath;

        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? "Resources";
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var assemblyName = resourceSource.Assembly.GetName().Name;
            var path = resourceSource.FullName;

            return Create(resourceSource.Assembly.GetName().Name, path);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer($"");
        }
    }
}
