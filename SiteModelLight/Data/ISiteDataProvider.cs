using System.Collections.Generic;

namespace SiteModelLight
{
    public interface ISiteDataProvider
    {
        List<Options<string>> GetOptionalsList(params string[] keys);
        Option<string>[] GetOptionals(string key);

        System.IO.Stream LoadXmlData();

        void SaveDataToLocal(Dictionary<string, SiteModel> models);
        Dictionary<string, SiteModel> LoadDataLocal();
    }
}
