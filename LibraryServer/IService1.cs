﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LibraryServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        double ConvertDkkToEur(double value);

        [OperationContract]
        double GetExchangeRate(string iso);

        [OperationContract]
        Currency[] GetCurrencyInfo();

        [OperationContract]
        double CalculateValue(string isoFrom, string isoTo, double value);

        [OperationContract]
        bool ChangeExchangerate(string iso, double value);

        [OperationContract]
        bool CreateCurrency(string name, string iso, double exchangeRate);


        [OperationContract]
        History GetHistory();

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "LibraryServer.ContractType".

    [DataContract]
    public class CurrencyRepo
    {
        private static CurrencyRepo instance = null;
        public static CurrencyRepo Instance {
            get
            {
                if (instance == null)
                {
                    instance = new CurrencyRepo();
                }
                return instance;
            }
        }
        Dictionary<string, Currency> currencies = new Dictionary<string, Currency>();

        private CurrencyRepo()
        {
            currencies.Add("USD", new Currency("Amerika", "USD", 5.240200));
            currencies.Add("CAD", new Currency("Canada", "CAD", 4.922700));
            currencies.Add("EUR", new Currency("Euro", "EUR", 7.459900));
            currencies.Add("NOK", new Currency("Norge", "NOK", 9.03400));
            currencies.Add("GBP", new Currency("Storbritannien", "GBP", 9.475300));
            currencies.Add("SEK", new Currency("Sverige", "SEK", 7.82100));
        }

        public bool AddCurrency(Currency c)
        {
            int prevCount = currencies.Count;
            currencies.Add(c.Name, c);

            return prevCount < currencies.Count;
        }

        public bool ChangeRate(string iso, double value)
        {
            currencies[iso].ExchangeRate = value;

            return currencies[iso].ExchangeRate == value;
        }

        public Currency[] GetInfo()
        {
            return currencies.Values.ToArray();
        }

        public Dictionary<string,Currency> GetDictionary()
        {
            return currencies;
        }

    }

    [DataContract]
    public class Currency
    {

        string name;
        string iso;
        double exchangeRate;

        public Currency(string n, string i, double e)
        {
            name = n;
            iso = i;
            exchangeRate = e;
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Iso
        {
            get { return iso; }
            set { iso = value; }
        }

        [DataMember]
        public double ExchangeRate
        {
            get { return exchangeRate; }
            set { exchangeRate = value; }
        }
    }

    [DataContract]
    public class History
    {
        private static History instance = null;
        public static History Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new History();
                }
                return instance;
            }
        }
        List<double> exchangeRates = new List<double>();
        List<double> amounts = new List<double>();

        [DataMember]
        public double[] ExchangeRates
        {
            get { return exchangeRates.ToArray(); }
        }

        [DataMember]
        public double[] Amounts
        {
            get { return amounts.ToArray(); }
        }
        
        public void AddExchangeRate(double er)
        {
            exchangeRates.Add(er);
        }
        
        public void AddAmount(double amount)
        {
            amounts.Add(amount);
        }
    }
}
