using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Extensions.Localization.Json
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _resourcesCache = new ConcurrentDictionary<string, Dictionary<string, string>>();
        private readonly string _resourcesPath;
        private readonly string _resourceName;
        private readonly ResourcesPathType _resourcesPathType;
        private readonly ILogger _logger;

        private string _searchedLocation;

        public JsonStringLocalizer(
            JsonLocalizationOptions localizationOptions,
            string resourceName,
            ILogger logger)
        {
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

            var culture = CultureInfo.CurrentUICulture;
            string value = null;

            BuildResourcesCache(culture.Name);

            if (_resourcesCache.TryGetValue(culture.Name, out var resources) && resources?.ContainsKey(name) == true)
            {
                value = resources[name];
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
            BuildResourcesCache(culture.Name);

            if (_resourcesCache.TryGetValue(culture.Name, out var resources))
            {
                foreach (var resource in resources)
                {
                    yield return resource.Key;
                }
            }
            else
            {
                yield return null;
            }
        }

        private void BuildResourcesCache(string culture)
        {
            _resourcesCache.GetOrAdd(culture, _ =>
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
                    var content = File.ReadAllText(_searchedLocation);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            value = content.Trim().JsonToType<Dictionary<string, string>>();
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e, $"invalid json content, path: {_searchedLocation}, content: {content}");
                        }
                    }
                }

                return value;
            });
        }
    }
}
