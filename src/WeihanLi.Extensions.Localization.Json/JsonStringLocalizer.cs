using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Extensions.Localization.Json
{
    internal sealed class JsonStringLocalizer : IStringLocalizer
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _resourcesCache = new();
        private readonly string _resourcesPath;
        private readonly JsonLocalizationOptions _localizationOptions;
        private readonly string _resourceName;
        private readonly ResourcesPathType _resourcesPathType;
        private readonly ILogger _logger;

        private string _searchedLocation;

        public JsonStringLocalizer(
            JsonLocalizationOptions localizationOptions,
            string resourceName,
            ILogger logger)
        {
            _localizationOptions = localizationOptions;
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            _logger = logger ?? NullLogger.Instance;
            _resourcesPath = Path.Combine(ApplicationHelper.AppRoot, localizationOptions.ResourcesPath);
            _resourcesPathType = localizationOptions.ResourcesPathType;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetStringSafely(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetStringSafely(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _searchedLocation);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

        public IStringLocalizer WithCulture(CultureInfo culture) => this;

        private IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var resourceNames = includeParentCultures
                ? GetAllStringsFromCultureHierarchy(culture)
                : GetAllResourceStrings(culture);

            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
            }
        }

        private string GetStringSafely(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;

            var resources = GetResources(CultureInfo.CurrentUICulture.Name);
            if (resources?.TryGetValue(name, out var resource) is true)
            {
                value = resource;
            }

            return value;
        }

        private IEnumerable<string> GetAllStringsFromCultureHierarchy(CultureInfo startingCulture)
        {
            var currentCulture = startingCulture;
            var resourceNames = new HashSet<string>();

            while (currentCulture.Equals(currentCulture.Parent) == false)
            {
                var cultureResourceNames = GetAllResourceStrings(currentCulture);

                if (cultureResourceNames != null)
                {
                    foreach (var resourceName in cultureResourceNames)
                    {
                        resourceNames.Add(resourceName);
                    }
                }

                currentCulture = currentCulture.Parent;
            }

            return resourceNames;
        }

        private IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
        {
            var resources = GetResources(culture.Name);
            return resources?.Select(r => r.Key);
        }

        private Dictionary<string, string> GetResources(string culture)
        {
            return _resourcesCache.GetOrAdd(culture, _ =>
            {
                var resourceFile = "json";
                if (_resourcesPathType == ResourcesPathType.TypeBased)
                {
                    resourceFile = $"{culture}.json";
                    if (_resourceName != null)
                    {
                        resourceFile = string.Join(".", _resourceName.Replace('.', Path.DirectorySeparatorChar), resourceFile);
                    }
                }
                else
                {
                    resourceFile = string.Join(".",
                        Path.Combine(culture, _resourceName.Replace('.', Path.DirectorySeparatorChar)), resourceFile);
                }

                _searchedLocation = Path.Combine(_resourcesPath, resourceFile);
                Dictionary<string, string> value = null;

                if (File.Exists(_searchedLocation))
                {
                    try
                    {
                        if (_localizationOptions.JsonSerializerType == JsonSerializerType.NewtonsoftJson)
                        {
                            var content = File.ReadAllText(_searchedLocation, System.Text.Encoding.UTF8);
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                value = content.Trim()
                                        .JsonToObject<Dictionary<string, string>>()
                                    ;
                            
                            }
                        }
                        else if (_localizationOptions.JsonSerializerType == JsonSerializerType.SystemTextJson)
                        {
                            using var stream = File.OpenRead(_searchedLocation);
                            value = JsonSerializer.Deserialize<Dictionary<string, string>>(stream, DefaultJsonSerializerOptions);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to get json content, path: {path}, {jsonSerializerType}", _searchedLocation, _localizationOptions.JsonSerializerType);
                    }
                }

                return value;
            });
        }
    }
}
