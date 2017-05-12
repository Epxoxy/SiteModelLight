using System;
using System.Collections.Generic;
using System.IO;
using SiteModelLight;

namespace ConsoleApp
{
    class ISiteDataProviderImpl : ISiteDataProvider
    {
        public Option<string>[] GetOptionals(string key)
        {
            throw new NotImplementedException();
        }

        public List<Options<string>> GetOptionalsList(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, SiteModel> LoadDataLocal()
        {
            throw new NotImplementedException();
        }

        public Stream LoadXmlData()
        {
            throw new NotImplementedException();
        }

        public void SaveDataToLocal(Dictionary<string, SiteModel> models)
        {
            throw new NotImplementedException();
        }
    }
}
