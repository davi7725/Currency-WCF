using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        Dictionary<string, Currency> currencies = GetCurrenciesFromDatabase();


        private static Dictionary<string, Currency> GetCurrenciesFromDatabase()
        {
            Dictionary<string, Currency> tmpCurrencies = new Dictionary<string, Currency>();
            SqlConnection conn = OpenConnection();
            SqlCommand comm = new SqlCommand("SELECT * FROM Currency", conn);
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string iso = reader.GetString(0);
                    decimal exchangeRate = reader.GetDecimal(1);
                    tmpCurrencies.Add(iso, new Currency(iso, iso, Convert.ToDouble(exchangeRate)));
                }
            }
            conn.Close();
            conn.Dispose();

            return tmpCurrencies;
        }

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
            SqlConnection conn = OpenConnection();
            bool locked = GetLock(iso, conn);

            if (locked == false)
            {
                SqlCommand comm = new SqlCommand("UPDATE Currency SET exchangeRate = " + value + " WHERE iso='" + iso + "'", conn);
                int nr = comm.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                if (nr == 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public double ConvertDkkToEur(double value)
        {
            AddToHistory("EUR", value);
            return value / currencies["EUR"].ExchangeRate;
        }

        public bool CreateCurrency(string name, string iso, double exchangeRate)
        {
            SqlConnection conn = OpenConnection();
            SqlCommand comm = new SqlCommand("INSERT INTO Currency VALUES ('"+iso+"',"+exchangeRate+",0)", conn);
            int nr = comm.ExecuteNonQuery();

            conn.Close();
            conn.Dispose();

            if (nr == 0)
                return false;
            else
                return true;
        }

        public Currency[] GetCurrencyInfo()
        {
            List<Currency> tmpCurr = new List<Currency>();
            foreach(Currency c in currencies.Values)
            {
                tmpCurr.Add(c);
            }
            return tmpCurr.ToArray();
        }

        public double GetExchangeRate(string iso)
        {
            return currencies[iso].ExchangeRate;
        }

        public void Lock(string iso)
        {
            SqlConnection conn = OpenConnection();
            SqlCommand comm = new SqlCommand("UPDATE Currency SET lock = 1 WHERE iso='" + iso + "'", conn);
            comm.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void UnLock(string iso)
        {
            SqlConnection conn = OpenConnection();
            SqlCommand comm = new SqlCommand("UPDATE Currency SET lock = 0 WHERE iso='" + iso + "'", conn);
            comm.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        private bool GetLock(string iso , SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand("SELECT lock FROM Currency WHERE iso='"+iso+"'", conn);
            SqlDataReader reader = comm.ExecuteReader();
            bool locked = false;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    locked = reader.GetBoolean(0);
                }
            }
            reader.Close();
            comm.Dispose();

            return locked;
        }

        private static SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection("Server = ealdb1.eal.local; Database = EJL69_DB; User ID = ejl69_usr; Password = Baz1nga69");
            conn.Open();
            return conn;
        }
    }
}
