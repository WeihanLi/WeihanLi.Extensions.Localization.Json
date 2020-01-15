using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Extensions.Localization.Json
{
    internal class JsonStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _dic = new ConcurrentDictionary<string, Dictionary<string, string>>();

        public JsonStringLocalizer(JsonLocalizationOptions localizationOptions, string resourceName)
        {
            var path = Path.Combine(ApplicationHelper.AppRoot, localizationOptions.ResourcesPath, resourceName);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public LocalizedString this[string name] => throw new NotImplementedException();

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
    }
}
