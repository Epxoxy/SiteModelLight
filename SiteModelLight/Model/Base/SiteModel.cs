using System.Collections.Generic;

namespace SiteModelLight
{
    public class SiteModel
    {
        public string SiteName { get; set; }
        public string EncodingName { get; set; }
        public UnitModel LoginModel { get; set; }
        public UnitModel LogoutModel { get; set; }

        public string LoginUrl => LoginModel == null ? null : LoginModel.Url;
        public string LogoutUrl => LogoutModel == null ? null : LogoutModel.Url;

        public Dictionary<string, string> Patterns { get; set; }
        public Dictionary<string, UnitModel> Units { get; set; }
        public Dictionary<string, WebPageModel> WebPageModels { get; set; }

        public WebPageModel GetWebPageModel(string key)
        {
            if (WebPageModels == null || !WebPageModels.ContainsKey(key)) return null;
            return WebPageModels[key];
        }

        public string GetPattern(string key)
        {
            if (Patterns == null || !Patterns.ContainsKey(key)) return null;
            return Patterns[key];
        }
    }
}
