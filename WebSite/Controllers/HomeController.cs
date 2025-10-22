using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text;
using WebSite.Data;
using WebSite.Data.Models;
using WebSite.Dtos;
using WebSite.Extensions;
using WebSite.Services.Interfaces;

namespace WebSite.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
        IGeoLocationService geoLocationService,
        AppDbContext context,
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        IMemoryCache cache) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IGeoLocationService _geoLocationService = geoLocationService;
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IMemoryCache _cache = cache;

        [HttpGet]
        [EnableRateLimiting("ip_based_limiter")]
        public async Task<IActionResult> Index([FromQuery] string? gcLid, [FromQuery] string? secretKey, CancellationToken cancellationToken = default)
        {
            var webSiteName = _configuration["WebSiteName"];
            var key = _configuration["StringEncryptionKey"]!;

            if (webSiteName == null)
            {
                return View(new IndexDto());
            }

            var webSite = await _context.WebSites
                .Where(w => w.Name.ToLower() == webSiteName.ToLower())
                .Include(w => w.MarketSubcatagory)
                .ThenInclude(msc => msc.Market)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(secretKey))
            {
                if (webSite == null)
                {
                    return View(new IndexDto());
                }

                if (webSite.MarketSubcatagory is null || !webSite.MarketSubcatagory.BlackSideStatus)
                {
                    return View(new IndexDto());
                }
            }

            var mscId = webSite?.MarketSubcatagory?.Id;

            if (!string.IsNullOrEmpty(secretKey))
            {
                var dbSecretKey = await _context.StaticDatas.FirstAsync(s => s.Key == "SecretKey", cancellationToken);

                if (secretKey.Equals(dbSecretKey.Value))
                {
                    var brands = await GetBrandsAsync(webSite.MarketSubcatagory.Id, webSite.Id, cancellationToken);

                    brands = brands.Select(brand => { brand.Url = BuildEncryptedGotoUrl(brand, "1001", key); return brand; }).ToList();

                    IndexDto dto = new IndexDto
                    {
                        Status = true,
                        Brands = brands
                    };

                    return View("Black", dto);
                }
                else
                {
                    return View(new IndexDto());
                }
            }

            var userAgent = Request.Headers["User-Agent"].ToString();
            var userAgentStatus = userAgent.IsHumanUser();

            var gCLid = await _context.AddGclidIfPresentAsync(gcLid, webSite, cancellationToken);

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            CountryInfo requestCountry = await _geoLocationService.GetCountryInfoAsync(userIp, cancellationToken);


            try
            {
                

                Log newLog = new()
                {
                    Ip = userIp,
                    UserAgent = userAgent,
                    CreateDate = DateTime.UtcNow.AddHours(4),
                    Country = requestCountry.Name,
                    CountryCode = requestCountry.Code,
                    GClidId = gCLid?.Id,
                    WebSiteId = webSite.Id,
                    MarketSubcatagoryId = webSite.MarketSubcatagory.Id,
                    IsBlack = (userAgentStatus && gCLid is not null && string.Equals(webSite?.MarketSubcatagory?.Market?.Code,
                        requestCountry?.Code,
                        StringComparison.OrdinalIgnoreCase))
                };

                _context.Logs.Add(newLog);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Log");
            }


            if (userAgentStatus && gCLid is not null && string.Equals(webSite?.MarketSubcatagory?.Market?.Code,
                    requestCountry?.Code,
                    StringComparison.OrdinalIgnoreCase))
            {
                var brands = await GetBrandsAsync(webSite.MarketSubcatagory.Id, webSite.Id, cancellationToken);

                brands = brands.Select(brand => { brand.Url = BuildEncryptedGotoUrl(brand, gCLid.Id.ToString(), key); return brand; }).ToList();

                IndexDto dto = new IndexDto
                {
                    Status = true,
                    Brands = brands
                };

                return View("Black", dto);
            }

            return View(new IndexDto());
        }

        [HttpGet]
        [Route("ca")]
        [EnableRateLimiting("ip_based_limiter")]
        public async Task<IActionResult> Index2([FromQuery] string? gcLid, [FromQuery] string? secretKey, CancellationToken cancellationToken = default)
        {
            var webSiteName = _configuration["WebSiteName2"];
            var key = _configuration["StringEncryptionKey"]!;

            if (webSiteName == null)
            {
                return View(new IndexDto());
            }

            var webSite = await _context.WebSites
                .Where(w => w.Name.ToLower() == webSiteName.ToLower())
                .Include(w => w.MarketSubcatagory)
                .ThenInclude(msc => msc.Market)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrEmpty(secretKey))
            {
                if (webSite == null)
                {
                    return View(new IndexDto());
                }

                if (webSite.MarketSubcatagory is null || !webSite.MarketSubcatagory.BlackSideStatus)
                {
                    return View(new IndexDto());
                }
            }

            var mscId = webSite?.MarketSubcatagory?.Id;

            if (!string.IsNullOrEmpty(secretKey))
            {
                var dbSecretKey = await _context.StaticDatas.FirstAsync(s => s.Key == "SecretKey", cancellationToken);

                if (secretKey.Equals(dbSecretKey.Value))
                {
                    var brands = await GetBrandsAsync(webSite.MarketSubcatagory.Id, webSite.Id, cancellationToken);

                    brands = brands.Select(brand => { brand.Url = BuildEncryptedGotoUrl(brand, "1001", key); return brand; }).ToList();

                    IndexDto dto = new IndexDto
                    {
                        Status = true,
                        Brands = brands
                    };

                    return View("Black", dto);
                }
                else
                {
                    return View(new IndexDto());
                }
            }

            var userAgent = Request.Headers["User-Agent"].ToString();
            var userAgentStatus = userAgent.IsHumanUser();

            var gCLid = await _context.AddGclidIfPresentAsync(gcLid, webSite, cancellationToken);

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            CountryInfo requestCountry = await _geoLocationService.GetCountryInfoAsync(userIp, cancellationToken);

                try
                {
                    

                    Log newLog = new()
                    {
                        Ip = userIp,
                        UserAgent = userAgent,
                        CreateDate = DateTime.UtcNow.AddHours(4),
                        Country = requestCountry.Name,
                        CountryCode = requestCountry.Code,
                        GClidId = gCLid?.Id,
                        WebSiteId = webSite.Id,
                        MarketSubcatagoryId = webSite.MarketSubcatagory.Id,
                        IsBlack = (userAgentStatus && gCLid is not null && string.Equals(webSite?.MarketSubcatagory?.Market?.Code,
                            requestCountry?.Code,
                            StringComparison.OrdinalIgnoreCase))
                    };

                    _context.Logs.Add(newLog);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving Log");
                }
            

            if (userAgentStatus && gCLid is not null && string.Equals(webSite?.MarketSubcatagory?.Market?.Code,
                    requestCountry?.Code,
                    StringComparison.OrdinalIgnoreCase))
            {
                var brands = await GetBrandsAsync(webSite.MarketSubcatagory.Id, webSite.Id, cancellationToken);

                brands = brands.Select(brand => { brand.Url = BuildEncryptedGotoUrl(brand, gCLid.Id.ToString(), key); return brand; }).ToList();

                IndexDto dto = new IndexDto
                {
                    Status = true,
                    Brands = brands
                };

                return View("Black", dto);
            }

            return View(new IndexDto());
        }

        [HttpGet("url")]
        public async Task<IActionResult> GoTo([FromQuery] string c, CancellationToken cancellationToken)
        {
            var key = _configuration["StringEncryptionKey"]!;
            if (string.IsNullOrWhiteSpace(key))
                return StatusCode(500, "Configuration error.");

            try
            {
                string url = c.FromUrlSafeBase64();

                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
                {
                    return Ok();
                }

                var query = QueryHelpers.ParseQuery(uri.Query);

                int brandId = int.Parse(query["brandid"]!);
                int gCLid = int.Parse(query["gclid"]!);
                int webSiteId = int.Parse(query["webid"]!);
                int mscId = int.Parse(query["mscId"]!);
                int place = int.Parse(query["place"]!);

                try
                {
                    BrandClick brandClick = new()
                    {
                        BrandId = brandId,
                        WebSiteId = webSiteId,
                        MarketSubcatagoryId = mscId,
                        Place = place,
                        GClidId = gCLid,
                        CreateDate = DateTime.UtcNow.AddHours(4),
                    };

                    _context.BrandClicks.Add(brandClick);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return Redirect(url);
            }
            catch
            {
                return BadRequest("Invalid encoded data.");
            }
        }

        private async Task<List<BrandDto>> GetBrandsAsync(int mscId, int webSiteId, CancellationToken cancellationToken)
        {
            var cacheKey = $"brands_{mscId}_{webSiteId}";

            if (_cache.TryGetValue(cacheKey, out List<BrandDto> cachedBrands))
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Cache HIT: {cacheKey}, Count={cachedBrands.Count}");
                return CloneBrands(cachedBrands);
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Cache MISS: {cacheKey}, DB’den veri okunuyor...");

            var brands = await _context.Brands
                .AsNoTracking()
                .Where(b => b.MarketSubcatagoryId == mscId && b.IsActive)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Logo = b.Logo.Replace(" ", "_"),
                    Url = b.Link,
                    PaymentOptions = b.PaymentTypes
                                      .Select(pt => pt.Logo.Replace(" ", "_"))
                                      .ToList(),
                    Place = b.Place,
                    Option1 = b.Option1,
                    Option2 = b.Option2,
                    Option3 = b.Option3,
                    Description = b.Description,
                    PostBackType = b.PostBackType ?? PostBackType.Raketech,
                    WebSiteId = webSiteId,
                    MarketSubcatagoryId = b.MarketSubcatagoryId ?? 0,
                })
                .OrderBy(b => b.Place)
                .ToListAsync(cancellationToken);

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DB’den {brands.Count} brand okundu. Cache’e yaz?l?yor (Key={cacheKey}, TTL=5dk)");

            _cache.Set(cacheKey, brands, TimeSpan.FromMinutes(5));

            return CloneBrands(brands);
        }

        private List<BrandDto> CloneBrands(List<BrandDto> source)
        {
            var start = DateTime.Now;
            Console.WriteLine($"[{start:yyyy-MM-dd HH:mm:ss.fff}] CloneBrands ba?lad?. Kaynak listede {source.Count} item var.");

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var result = source.Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Logo = b.Logo,
                Url = b.Url,
                PaymentOptions = b.PaymentOptions.ToList(),
                Place = b.Place,
                Option1 = b.Option1,
                Option2 = b.Option2,
                Option3 = b.Option3,
                Description = b.Description,
                PostBackType = b.PostBackType,
                WebSiteId = b.WebSiteId,
                MarketSubcatagoryId = b.MarketSubcatagoryId
            }).ToList();

            sw.Stop();
            var end = DateTime.Now;
            Console.WriteLine($"[{end:yyyy-MM-dd HH:mm:ss.fff}] CloneBrands bitti. {result.Count} item kopyaland?. Süre={sw.ElapsedMilliseconds} ms");

            return result;
        }

        #region PrivateMethods

        private static string BuildEncryptedGotoUrl(
            BrandDto brand,
            string? gclidId,
            string key,
            string gotoPath = "/url?c=")
        {
            if (brand is null) throw new ArgumentNullException(nameof(brand));

            string link = brand.PostBackType switch
            {
                PostBackType.Affilika => AppendQuery(brand.Url ?? string.Empty, "visit_id", gclidId),
                PostBackType.Raketech => (brand.Url ?? string.Empty) + (gclidId ?? string.Empty),
                _ => brand.Url ?? string.Empty
            };

            link = AppendQuery(link, "brandid", brand.Id.ToString());
            link = AppendQuery(link, "gclid", gclidId);
            link = AppendQuery(link, "webid", brand.WebSiteId.ToString());
            link = AppendQuery(link, "mscId", brand.MarketSubcatagoryId.ToString());
            link = AppendQuery(link, "place", brand.Place.ToString());

            string encoded = link.ToUrlSafeBase64();

            // Normal Base64
            //byte[] cipherBytes = System.Text.Encoding.UTF8.GetBytes(cipher);
            //string base64 = Convert.ToBase64String(cipherBytes);

            // URL-safe Base64
            //string urlSafeBase64 = base64
            //    .Replace("+", "-")
            //    .Replace("/", "_")
            //    .TrimEnd('=');

            return $"{gotoPath}{encoded}";

            //return link;
        }

        private static string AppendQuery(string url, string name, string? value)
        {
            if (string.IsNullOrEmpty(value)) return url;
            char sep = url.Contains('?') ? '&' : '?';
            return $"{url}{sep}{name}={Uri.EscapeDataString(value)}";
        }

        #endregion PrivateMethods
    }
}

public static class Base64UrlExtensions
{
    public static string FromUrlSafeBase64(this string input)
    {
        string base64 = input
            .Replace("-", "+")
            .Replace("_", "/");

        // padding düzelt
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
}