using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Puma.DataLayer.BusinessEntity;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly IAppSettingRepository _appSettingRepository;

        //public AuthorizeAttribute(IAppSettingRepository appSettingRepository)
        //{
        //    _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _appSettingRepository = (IAppSettingRepository)context.HttpContext.RequestServices.
          GetService(typeof(IAppSettingRepository));

            if (Convert.ToBoolean(_appSettingRepository.GetAppSettingValue(AppSetting.IsAuthenticationApplicable).Result))
            {
                var user = (string)context.HttpContext.Items["UserName"];

                if (string.IsNullOrWhiteSpace(user))
                {
                    // not logged in
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }
    }
}
