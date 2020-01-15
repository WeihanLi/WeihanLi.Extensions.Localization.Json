namespace WeihanLi.Extensions.Localization.Json
{
    public class JsonLocalizationOptions
    {
        /// <summary>
        /// The relative path under application root where resource files are located.
        /// </summary>
        public string ResourcesPath { get; set; } = "Resources";

        /// <summary>
        /// ResourcesPathType
        /// </summary>
        public ResourcesPathType ResourcesPathType { get; set; }
    }

    public enum ResourcesPathType
    {
        TypeBased = 0,
        CultureBased = 1,
    }
}
