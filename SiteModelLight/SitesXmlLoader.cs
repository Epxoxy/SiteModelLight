using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SiteModelLight
{
    public class SitesXmlLoader
    {
        private ISiteDataProvider dataProvider = null;
        public SitesXmlLoader(ISiteDataProvider provider)
        {
            this.dataProvider = provider;
        }

        private string tryGet(XAttribute attribute, string defaultValue)
        {
            if (attribute == null) return defaultValue;
            return attribute.Value;
        }

        private bool isComment(XElement element)
        {
            if (element == null) return false;
            return element.Name.LocalName == "#comment";
        }

        //<page key url type menu>
        //   <unit ..... />
        //</page>
        private WebPageModel readPage(XElement element)
        {
            if (element == null || !element.HasAttributes || !element.HasElements)
                return null;
            var page = new WebPageModel();
            var itemName = element.Name.LocalName;
            if(itemName == "page")
            {
                var key = element.Attribute("key");
                if (key != null)
                {
                    page.Key = key.Value;
                    if (element != null && element.HasElements)
                    {
                        var cacheUnits = new Dictionary<string, UnitModel>();
                        foreach (var child in element.Elements())
                        {
                            var unit = readUnit(child);
                            if(unit != null)
                            {
                                cacheUnits.Add(unit.Key, unit);
                            }
                        }
                        if (cacheUnits.Count > 0)
                            page.Units = cacheUnits;
                    }
                    return page;
                }
            }
            return null;
        }

        //<unit key url type fetchFormat matchFormat headers>
        //   <options ...... />
        //</unit>
        private UnitModel readUnit(XElement element)
        {
            if (element.Name.LocalName == "unit" && element.HasAttributes)
            {
                var key = element.Attribute("key");
                if (key == null) return null;
                var unitModel = new UnitModel() { Key = key.Value };
                foreach (var unitAttr in element.Attributes())
                {
                    string value = unitAttr.Value;
                    if (string.IsNullOrEmpty(value)) continue;
                    switch (unitAttr.Name.LocalName)
                    {
                        case "type":
                            unitModel.Type = value;
                            break;
                        case "url":
                            unitModel.Url = value;
                            break;
                        case "fetchFormat":
                            unitModel.FetchFormat = value;
                            break;
                        case "matchFormat":
                            {
                                if (unitModel.MatchModel == null)
                                    unitModel.MatchModel = new MatchModel();
                                unitModel.MatchModel.Format = value;
                            }
                            break;
                        case "headers":
                            {
                                if (unitModel.MatchModel == null)
                                    unitModel.MatchModel = new MatchModel();
                                unitModel.MatchModel.Headers = value.Split('|');
                            }
                            break;
                    }
                }
                unitModel.OptionsList = readOptionsList(element);
                if (!string.IsNullOrEmpty(unitModel.FetchFormat)
                    || !string.IsNullOrEmpty(unitModel.Url)
                    || unitModel.MatchModel != null
                    || unitModel.OptionsList != null)
                    return unitModel;
            }
            return null;
        }
        
        //<options type url pattern description >
        //   <option ......../>
        //</options>
        private List<Options<string>> readOptionsList(XElement element)
        {
            if (element != null && element.HasElements)
            {
                var optionsList = new List<Options<string>>();
                foreach (var child in element.Elements())
                {
                    if (child.Name.LocalName == "options")
                    {
                        var options = new Options<string>();
                        if (child.HasAttributes)
                        {
                            foreach (var attr in child.Attributes())
                            {
                                var value = attr.Value;
                                switch (attr.Name.LocalName)
                                {
                                    case "type":
                                        var def = OptionType.Selection;
                                        Enum.TryParse<OptionType>(value, out def);
                                        options.Type = def;
                                        break;
                                    case "url":
                                        options.SourceUrl = value; break;
                                    case "pattern":
                                        options.Pattern = value; break;
                                    case "description":
                                        options.Description = value; break;
                                }
                            }
                        }
                        options.Items = readOptions(child);
                        optionsList.Add(options);
                    }
                }
                if (optionsList.Count > 0)
                    return optionsList;
            }
            return null;
        }

        //<option value display description />
        private Option<string>[] readOptions(XElement element)
        {
            if (element != null && element.HasElements)
            {
                List<Option<string>> options = new List<Option<string>>();
                foreach (var child in element.Elements())
                {
                    if (child.Name.LocalName == "option" && child.HasAttributes)
                    {
                        var option = new Option<string>();
                        foreach (var attr in child.Attributes())
                        {
                            var value = attr.Value;
                            switch (attr.Name.LocalName)
                            {
                                case "value":
                                    option.Value = value; break;
                                case "display":
                                    option.Display = value; break;
                                case "description":
                                    option.Description = value; break;
                            }
                        }
                        options.Add(option);
                    }
                }
                if (options.Count > 0)
                    return options.ToArray();
            }
            return null;
        }

        //<models>
        //  <site siteName encodingName>
        //    <page key>
        //        <unit key url type fetchFormat matchFormat headers>
        //           <options type url matchFormat description >
        //              <option value display description />
        //           </options>
        //        </unit>
        //    </page>
        //  </site>
        //</models>
        public bool Load(System.IO.Stream xmlStream, ref Dictionary<string, SiteModel> models)
        {
            if (dataProvider == null)
                return false;
            var siteModelsCache = new Dictionary<string, SiteModel>();
            #region Read xml and initalize dictionary
            try
            {
                #region Reading
                using (xmlStream)
                {
                    XElement rootNode = XElement.Load(xmlStream);
                    foreach (var childElement in rootNode.Elements())
                    {
                        //Initilize request dictionary
                        var webpageModels = new Dictionary<string, WebPageModel>();
                        var cacheUnits = new Dictionary<string, UnitModel>();
                        var patternDictionary = new Dictionary<string, string>();
                        UnitModel loginUnit = null;
                        UnitModel logoutUnit = null;
                        //Get basic attribute of site name and encoding name
                        string siteName = tryGet(childElement.Attribute("siteName"), "Common");
                        string encodingName = tryGet(childElement.Attribute("encodingName"), "utf-8");
                        foreach (var element in childElement.Elements())
                        {
                            if (isComment(element))
                                continue;
                            if (element.Name.LocalName == "pattern")
                            {
                                string key = string.Empty;
                                string pattern = string.Empty;
                                foreach (var attribute in element.Attributes())
                                {
                                    string value = attribute.Value;
                                    switch (attribute.Name.LocalName)
                                    {
                                        case "key": { key = value; } break;
                                        case "pattern": { pattern = value; } break;
                                    }
                                }
                                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(pattern))
                                    continue;
                                patternDictionary.Add(key, pattern);
                            }else if(element.Name.LocalName == "page")
                            {
                                var page = readPage(element);
                                if(page != null)
                                {
                                    System.Diagnostics.Debug.WriteLine(siteName + " -->" + page.Key);
                                    //Add to dictionary
                                    page.EncodingName = encodingName;
                                    webpageModels.Add(page.Key, page);
                                }
                            }else if(element.Name.LocalName == "unit")
                            {
                                var unit = readUnit(element);
                                if(unit != null)
                                {
                                    if (unit.Key == "login")
                                        loginUnit = unit;
                                    else if (unit.Key == "logout")
                                        logoutUnit = unit;
                                    cacheUnits.Add(unit.Key, unit);
                                }
                            }
                        }
                        //Initilize siteModel
                        var siteModel = new SiteModel
                        {
                            SiteName = siteName,
                            LoginModel = loginUnit,
                            LogoutModel = logoutUnit,
                            EncodingName = encodingName,
                            WebPageModels = webpageModels
                        };
                        if(cacheUnits.Count > 0)
                            siteModel.Units = cacheUnits;
                        if (patternDictionary.Count > 0)
                            siteModel.Patterns = patternDictionary;
                        //Add model to dictionary
                        siteModelsCache.Add(siteName, siteModel);
                    }
                }
                #endregion
                if(siteModelsCache.Count > 0)
                    models = siteModelsCache;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Initialize sites from xml fail with error\n " + e.Message + e.StackTrace);
                return false;
            }
            return siteModelsCache.Count > 0;
            #endregion
        }
    }
}
