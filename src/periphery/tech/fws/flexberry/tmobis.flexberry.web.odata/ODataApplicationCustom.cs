using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NewPlatform.Flexberry.ORM.ODataService.Middleware;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewPlatform.Flexberry.ORM.ODataService.Extensions;
/*
 *  TODO:
 *  Переопределенный класс из платформы https://github.com/Flexberry/NewPlatform.Flexberry.ORM.ODataService/blob/develop/NewPlatform.Flexberry.ORM.ODataService/Extensions/ODataApplicationBuilderExtensions.cs
 *  При выгрузке excel со списковой формы падает в ошибку
 *
 *  Response Content-Length mismatch: too many bytes written (3949 of 2597).
 *
 *  Переопределено поведение при выгрузке excel, тело ответа не перезаписывается
 */
public static class ODataApplicationCustom
{
    public static IApplicationBuilder UseODataServiceCustom(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes, int? maxTopValue = int.MaxValue)
    {
        if (app == null)
        {
            throw new ArgumentNullException("app");
        }
        VerifyODataServiceIsRegistered(app);
        app.Use(async delegate (HttpContext context, Func<Task> next)
        {
            await RewriteResponse(context, next);
        });
        return app.UseODataBatching().UseMiddleware<RequestHeadersHookMiddleware>(Array.Empty<object>()).UseMvc(delegate (IRouteBuilder builder)
        {
            builder.Select().Expand().Filter()
                .OrderBy()
                .MaxTop(maxTopValue)
                .Count();
        })
            .UseMvc(configureRoutes);
    }

    private static void VerifyODataServiceIsRegistered(IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetService(typeof(IPerRouteContainer)) == null)
        {
            throw new InvalidOperationException("IPerRouteContainer");
        }
    }

    public static async Task RewriteResponse(HttpContext context, Func<Task> next)
    {
        bool flag = Convert.ToBoolean(context.Request.Query["exportExcel"]) || context.Request.Path == "/api/fileload";
        if (flag)
        {
            await next();
        }
        else 
        {
            using MemoryStream responseBodyStream = new MemoryStream();
            Stream bodyStream = context.Response.Body;
            try
            {
                context.Response.Body = responseBodyStream;
                await next();
                responseBodyStream.Seek(0L, SeekOrigin.Begin);
                string responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                if (context.Response.ContentType != null && (context.Response.ContentType.Contains("application/json") || context.Response.ContentType.Contains("application/xml") || context.Response.ContentType.Contains("multipart/mixed")))
                {
                    responseBody = responseBody.Replace("(____.", "(").Replace("\"____.", "\"").Replace("____.", ".")
                        .Replace(" Namespace=\"____\"", " Namespace=\"\"");
                }
                using MemoryStream newStream = new MemoryStream();
                using StreamWriter sw = new StreamWriter(newStream);
                await sw.WriteAsync(responseBody);
                await sw.FlushAsync();
                newStream.Seek(0L, SeekOrigin.Begin);
                await newStream.CopyToAsync(bodyStream);
            }
            finally
            {
                context.Response.Body = bodyStream;
            }
        }
    }
}
