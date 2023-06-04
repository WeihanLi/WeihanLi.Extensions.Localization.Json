using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Extensions.Localization.Json
{
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache =
            new ConcurrentDictionary<string, JsonStringLocalizer>();

        private readonly ILoggerFactory _loggerFactory;
        private readonly JsonLocalizationOptions _localizationOptions;

        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
        {
            _localizationOptions = localizationOptions.Value;
            _loggerFactory = loggerFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }
            var resourceName = TrimPrefix(resourceSource.FullName, (_localizationOptions.RootNamespace ?? ApplicationHelper.ApplicationName) + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var resourceName = TrimPrefix(baseName, location + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        private JsonStringLocalizer CreateJsonStringLocalizer(string resourceName)
        {
            var logger = _loggerFactory.CreateLogger<JsonStringLocalizer>();
            System.Console.WriteLine("Looking for resource: {0}", resourceName);
            return _localizerCache.GetOrAdd(resourceName, resName => new JsonStringLocalizer(
                _localizationOptions,
                resName,
                logger));
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}
