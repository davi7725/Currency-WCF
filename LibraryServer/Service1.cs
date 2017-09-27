using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LibraryServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        Dictionary<string, Currency> currencies = CurrencyRepo.Instance.GetDictionary();
        History history = History.Instance;

        public void AddToHistory(string iso, double value)
        {
            history.AddExchangeRate(currencies[iso].ExchangeRate);
            history.AddAmount(value);
        }

        public History GetHistory()
        {
            return history;
        }

        public double CalculateValue(string isoFrom, string isoTo, double value)
        {

            double valueInDkk = value * currencies[isoFrom].ExchangeRate;

            AddToHistory(isoTo, value);

            return valueInDkk / currencies[isoTo].ExchangeRate;
        }

        public bool ChangeExchangerate(string iso, double value)
        {
            return CurrencyRepo.Instance.ChangeRate(iso, value);
        }

        public double ConvertDkkToEur(double value)
        {
            AddToHistory("EUR", value);
            return value / currencies["EUR"].ExchangeRate;
        }

        public bool CreateCurrency(string name, string iso, double exchangeRate)
        {
            return CurrencyRepo.Instance.AddCurrency(new Currency(name, iso, exchangeRate));
        }

        public Currency[] GetCurrencyInfo()
        {
            return CurrencyRepo.Instance.GetInfo();
        }

        public double GetExchangeRate(string iso)
        {
            return currencies[iso].ExchangeRate;
        }
    }
}
