using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace HundredSDK.Authentication.BasicTokens
{
    public class MemoryCacheHelper
    {
        private IMemoryCache cache;
        private TimeSpan expiration;
        public MemoryCacheHelper(IMemoryCache cache, TimeSpan expiration) {
            this.cache = cache;
            this.expiration = expiration;
        }

        public object Get(string key) {
            return this.cache.Get(key);
        }

        public void Add(string key, object value) {
            if (this.Exists(key)) {
                this.Delete(key);
            }
            this.cache.Set(key, value, this.expiration);
        }

        public void Delete(string key) {
            this.cache.Remove(key);
        }

        public bool Exists(string key) {
            object valor;
            return this.cache.TryGetValue(key, out valor);
        }
    }
}
