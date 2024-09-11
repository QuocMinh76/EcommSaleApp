namespace EcommSale.Utilities
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "d859b2f38baeb9cf47d56825";
        private readonly string _apiUrl = "https://v6.exchangerate-api.com/v6/d859b2f38baeb9cf47d56825/latest/VND";

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetExchangeRateFromAPIAsync(string baseCurrency, string targetCurrency)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}?base={baseCurrency}");
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to fetch exchange rate.");
			}

			var jsonString = await response.Content.ReadAsStringAsync();

            var rates = System.Text.Json.JsonDocument.Parse(jsonString).RootElement.GetProperty("conversion_rates");

            if (rates.TryGetProperty(targetCurrency, out var rateElement))
            {
                return rateElement.GetDecimal();
            }
            throw new Exception("Currency not found.");
        }
    }
}
