using MaxMind.GeoIP2;
using System.Text.Json;
using WebSite.Dtos;
using WebSite.Services.Interfaces;

namespace WebSite.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly string _mmdbPath;

        public GeoLocationService(IWebHostEnvironment environment)
        {
            _mmdbPath = Path.Combine(environment.WebRootPath, "Country.mmdb");
        }

        public async Task<CountryInfo> GetCountryInfoAsync(string ipAddress, CancellationToken ct)
        {
            CountryInfo countryInfo = new CountryInfo();
            try
            {
                string apiKey = "b10c26b12d706ffac2dd71c351e996c20e28e3566cb248a0ae668b7f"; // API anahtarınızı buraya ekleyin
                string url = $"https://api.ipdata.co/{ipAddress}?api-key={apiKey}";

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var ipDataResponse = JsonSerializer.Deserialize<IpDataResponse>(jsonResponse);

                        countryInfo.Name = ipDataResponse.country_name;
                        countryInfo.Code = ipDataResponse.country_code;
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync($"API Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return countryInfo;
        }

        // İpData.co yanıt modeli
        public class IpDataResponse
        {
            public string country_name { get; set; }
            public string country_code { get; set; }
        }
    }
}