using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_WPF.Core
{
    public interface IDatabaseAPI
    {
        bool isNameEditAllowed { get; }
        bool isRateEditAllowed { get; }

        List<Currency> Currencies { get; }

        double GetExchangeRate(string srcCrnc, string trgCrnc);
        List<string> GetCurrenciesList();
        void Rewrite(List<Currency> currencies);
    }
}
