using System.Collections.Generic;

namespace SiteModelLight
{
    public class WebPageModel
    {
        public string Key { get; set; }
        public string EncodingName { get; set; }
        public Dictionary<string, UnitModel> Units { get; set; }
        
        public static WebPageModel CreatePageModel(string key, string encodingName)
        {
            return new WebPageModel
            {
                Key = key,
                EncodingName = encodingName
            };
        }

        public static WebPageModel CreatePageModel(string key,string encodingName, Dictionary<string, UnitModel> units)
        {
            return new WebPageModel
            {
                Key = key,
                EncodingName = encodingName,
                Units = units
            };
        }
    }
}
