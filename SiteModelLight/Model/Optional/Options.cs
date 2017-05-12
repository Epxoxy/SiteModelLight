namespace SiteModelLight
{
    public class Options<T>
    {
        public string Key { get; set; }
        public OptionType Type { get; set; }
        //current value
        public T Value { get; set; }
        //option items
        public Option<T>[] Items { get; set; }
        //url for get options
        public string SourceUrl { get; set; }
        //for reading options from page when get data
        public string Pattern { get; set; }
        //description
        public string Description { get; set; }

        public Options()
        {

        }

        public Options(string key, OptionType type, Option<T>[] items, string description)
        {
            this.Key = key;
            this.Type = type;
            this.Items = items;
            this.Description = description;
        }
    }
}
