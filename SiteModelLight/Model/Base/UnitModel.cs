namespace SiteModelLight
{
    public class UnitModel
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string FetchFormat { get; set; }
        public MatchModel MatchModel { get; set; }
        public System.Collections.Generic.List<Options<string>> OptionsList { get; set; }
    }
}
