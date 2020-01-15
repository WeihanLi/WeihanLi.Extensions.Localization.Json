namespace WeihanLi.Extensions.Localization.Json
{
    public class JsonLocalizationOptions
    {
        /// <summary>
        /// The relative path under application root where resource files are located.
        /// </summary>
        public string ResourcesPath { get; set; } = "Resources";

        public ResourcesType ResourcesType { get; set; }
    }

    public enum ResourcesType
    {
        TypeBased = 0,
        CultureBased = 1,
    }
}
