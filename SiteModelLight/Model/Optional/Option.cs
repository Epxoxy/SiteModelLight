namespace SiteModelLight
{
    public class Option<T>
    {
        public string Description { get; set; }
        public string Display { get; set; }
        public T Value { get; set; }

        public Option()
        {

        }

        public Option(string display, T value)
        {
            Display = display;
            Value = value;
        }

        public Option(string display, string description, T value)
        {
            Display = display;
            Description = description;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Display}]={Value}]({Description})";
        }
    }
}
