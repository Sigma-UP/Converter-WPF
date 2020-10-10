using StringExtension;


namespace Converter_WPF
{
    public class Currency
    {
        public string Name { get; set; }
        public double Rate { get; set; }

        public Currency() { }

        public Currency(string Name, double Rate)
        {
            this.Name = Name;
            this.Rate = Rate;
        }

        public bool isValid()
        {
            if (this.Name.isUpper() && this.Name.isUpper() && this.Name.Length == 3 && this.Rate.ToString().isNumber()) 
                return true;
            return false;
        }
    }
}
