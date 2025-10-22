using WebSite.Dtos;

namespace WebSite.Services.Interfaces
{
    public interface IGeoLocationService
    {
        Task<CountryInfo> GetCountryInfoAsync(string ipAddress, CancellationToken ct);
    }
}