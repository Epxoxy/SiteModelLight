using System.Collections.Generic;

namespace SiteModelLight
{
    public class SitesProvider
    {
        private Dictionary<string, SiteModel> sitesModel = null;

        private SitesProvider() { }

        public SitesProvider(ISiteDataProvider provider)
        {
            var data = provider.LoadDataLocal();
            if (data == null)
            {
                new SitesXmlLoader(provider).Load(provider.LoadXmlData(), ref sitesModel);
                if (sitesModel != null)
                    provider.SaveDataToLocal(sitesModel);
            }
            else sitesModel = data;
        }

        public void Reload(ISiteDataProvider provider)
        {
            new SitesXmlLoader(provider).Load(provider.LoadXmlData(), ref sitesModel);
            if (sitesModel != null)
                provider.SaveDataToLocal(sitesModel);
        }
        
        public SiteModel GetSiteModel(string key)
        {
            if (sitesModel == null || !sitesModel.ContainsKey(key)) return null;
            return sitesModel[key];
        }
    }
}
