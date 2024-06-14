//using System;
//using System.Web;
//using System.Web.Mvc;

//namespace Store.Services
//{
//    public class JwtAuthenticationAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var request = filterContext.HttpContext;
//            var token = request.Cookies["jwt"]?.Value;

//            if (string.IsNullOrEmpty(token))
//            {
//                filterContext.Result = new HttpUnauthorizedResult("Token is null or empty");
//                return;
//            }

//            var userName = Authentication.ValidateToken(token);
//            if (string.IsNullOrEmpty(userName))
//            {
//                filterContext.Result = new HttpUnauthorizedResult("No username found or token invalid");
//                return;
//            }

//            base.OnActionExecuting(filterContext);
//        }
//    }
//}
