using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Converter_WPF
{
    public class Currency
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }

        public Currency() { }

        public Currency(int ID, string Name, double Rate)
        {
            this.ID = ID;
            this.Name = Name;
            this.Rate = Rate;
        }


    }
}
