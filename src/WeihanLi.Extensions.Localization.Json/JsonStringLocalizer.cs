using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace WeihanLi.Extensions.Localization.Json
{
    internal class JsonStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, string> _dic;

        public JsonStringLocalizer(string path)
        {
            var content = File.ReadAllText(path);
            if (string.IsNullOrEmpty(content))
            {
                _dic = new Dictionary<string, string>();
            }
            else
            {
                _dic = content.JsonToType<Dictionary<string, object>>().ToDictionary(x => x.Key, x => x.Value.ToString());
            }
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
