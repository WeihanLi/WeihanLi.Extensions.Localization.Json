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
            return new JsonStringLocalizer(_localizationOptions, TrimStart(ApplicationHelper.AppRoot, resourceSource.FullName));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_localizationOptions, TrimStart(location, baseName));
        }

        private static string TrimStart(string str, string start)
        {
            if (str?.StartsWith(start, StringComparison.Ordinal) == true)
            {
                return str.Substring(start.Length);
            }

            return str;
        }
    }
}
