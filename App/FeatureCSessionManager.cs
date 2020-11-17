using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;
using System;

namespace DotNetDay_201119
{
    public class FeatureCSessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public FeatureCSessionManager(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Task SetAsync(string featureName, bool enabled)
        {
            if(featureName != "FeatureC") { return Task.CompletedTask; }

            var session = _contextAccessor.HttpContext.Session;
            var sessionKey = $"feature_{featureName}";
            session.Set(sessionKey, new[] {enabled ? (byte) 1 : (byte) 0});

            return Task.CompletedTask;
        }

        public Task<Nullable<Boolean>> GetAsync(string featureName)
        {
            if(featureName != "FeatureC") 
            {
                return Task.FromResult<Nullable<Boolean>>(null);
            }

            var session = _contextAccessor.HttpContext.Session;
            var sessionKey = $"feature_{featureName}";
            if (session.TryGetValue(sessionKey, out var enabledBytes))
            {
                return Task.FromResult<Nullable<Boolean>>(enabledBytes[0] == 1);
            }

            return Task.FromResult<Nullable<Boolean>>(null);
        }
    }
}