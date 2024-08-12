using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ticketera.Providers
{
    public class Config : IConfig, ISingletonDependency
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _appConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Config(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IConfiguration appConfiguration)
        {
            _hostingEnvironment = env;
            _httpContextAccessor = httpContextAccessor;
            _appConfiguration = appConfiguration;
        }

        public string CurrentDomain
        {
            get
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var url = new Uri(string.Concat(request.Scheme,
                        "://",
                        request.Host.ToUriComponent(),
                        request.PathBase.ToUriComponent(),
                        request.Path.ToUriComponent(),
                        request.QueryString.ToUriComponent()));

                return url.AbsolutePath.Length > 1 ? url.AbsoluteUri.ToLower().Replace(url.AbsolutePath.ToLower(), "").Split("?")[0] : url.AbsoluteUri.ToLower();
            }
        }

        public bool IsDebug => !_hostingEnvironment.IsProduction();

        public string Get(string key)
        {
            return _appConfiguration[key];
        }

        public string GetConnetionString()
        {
            return _appConfiguration.GetConnectionString("Default");
        }

        public string MapPath(string path)
        {
            if (path.StartsWith("http")) return "";

            var paths = new List<string> { _hostingEnvironment.ContentRootPath };
            paths.AddRange(path.Split("/").Where(x => !x.IsNullOrEmpty()));
            var pa = Path.Combine(paths.ToArray());

            return pa;
        }
    }
}
