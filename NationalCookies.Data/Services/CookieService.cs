using Microsoft.Extensions.Caching.Distributed;
using NationalCookies.Data.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace NationalCookies.Data.Services
{
    public class CookieService : ICookieService
    {
        private CookieContext _context;
        private readonly IDistributedCache _cache;

        public CookieService(CookieContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<Cookie> GetAllCookies()
        {
            List<Cookie> cookies;
            var cachedCookies = _cache.GetString("cookies");
            if (!string.IsNullOrEmpty(cachedCookies))
            {
                cookies = JsonConvert.DeserializeObject<List<Cookie>>(cachedCookies);
            }
            else
            {
                //Get the cookies from the database
                cookies = _context.Cookies.ToList();

                //Implement cache expiration
                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(new System.TimeSpan(0, 60, 0));

                //Put it in cache
                _cache.SetString("cookies", JsonConvert.SerializeObject(cookies), options);
            }
            return cookies;
        }

        public void ClearCache()
        {
            _cache.Remove("cookies");
        }
    }
}
