using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_WPF.Core
{
    public interface IDatabaseAPI
    {
        List<Currency> Currencies { get; }

        double GetExchangeRate(string srcCrnc, string trgCrnc);
        List<string> GetCurrenciesList();
        void Save();
    }
}
