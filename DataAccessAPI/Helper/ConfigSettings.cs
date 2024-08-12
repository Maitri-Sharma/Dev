
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.IO;

namespace DataAccessAPI.Helper
{
    public static class ConfigSettings
    {
        private static string _connectionString = string.Empty;

        //private static string _jwtSecret = string.Empty;
        public static string GetConnectionString
        {
            get
            {
                var HttpContext = new Microsoft.AspNetCore.Http.HttpContextAccessor().HttpContext;
                var ServiceProvider = HttpContext.RequestServices.GetService<IAppSettingRepository>();
                
                if (string.IsNullOrWhiteSpace(_connectionString))
                    _connectionString = ServiceProvider.GetConnectionString().Result;

                return _connectionString;
            }
        }



        //public static string GetJWTSecret
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(_jwtSecret))
        //            _jwtSecret = new AppSettings().JWTSecret;

        //        return _jwtSecret;
        //    }
        //}
    }


}
