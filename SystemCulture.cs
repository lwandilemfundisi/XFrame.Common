namespace XFrame.Common
{
    public class SystemCulture
    {
        public SystemCulture()
        {

        }

        public SystemCulture(string countryCode, string currencySymbol, string currencyCode)
        {
            CountryCode = countryCode;
            CurrencySymbol = currencySymbol;
            CurrencyCode = currencyCode;
        }

        public string CountryCode { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrencyCode { get; set; }

        public static SystemCulture Default()
        {
            return new SystemCulture
            {
                CountryCode = "RSA",
                CurrencySymbol = "R",
                CurrencyCode = "ZAR"
            };
        }
    }
}
