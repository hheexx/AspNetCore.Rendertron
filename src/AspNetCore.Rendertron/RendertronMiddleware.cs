﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Rendertron
{
    public class RendertronMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRendertronClient _rendertronClient;
        private readonly IOptionsMonitor<RendertronOptions> _optionsAccessor;

        public RendertronMiddleware(
            RequestDelegate next,
            IRendertronClient rendertronClient,
            IOptionsMonitor<RendertronOptions> optionsAccessor)
        {
            _next = next;
            _rendertronClient = rendertronClient;
            _optionsAccessor = optionsAccessor;
        }

        public Task Invoke(HttpContext context)
        {
            var options = _optionsAccessor.CurrentValue;

            if (IsNeedRender(context, options))
            {
                return InvokeRender(context, options);
            }
            else
            {
                AddHeaders(context.Response, options, false);
                return _next(context);
            }
        }

        private bool IsNeedRender(HttpContext context, RendertronOptions options)
        {
            var userAgent = context.Request.Headers["User-agent"].ToString().ToLowerInvariant();
            var path = context.Request.Path.Value;

            if (options.ExtensionBlacklist.Any( ext => path.EndsWith("." + ext ))) {
                return false;
            }

            if (options.PathPrefixBlacklist.Any(prefix => path.StartsWith(prefix))) {
                return false;
            }

            return options.UserAgents.Any(x => userAgent.Contains(x.ToLowerInvariant()));
        }

        private async Task InvokeRender(HttpContext context, RendertronOptions options)
        {
            var cancellationToken = context.RequestAborted;

            var response = await _rendertronClient
                .RenderAsync(context.Request.GetDisplayUrl(), cancellationToken)
                .ConfigureAwait(false);

            AddHttpCacheHeaders(context.Response, options);
            AddHeaders(context.Response, options, true);

            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsync(response.Result, cancellationToken);
        }

        private void AddHttpCacheHeaders(HttpResponse httpResponse, RendertronOptions options)
        {
            if (options.HttpCacheMaxAge > TimeSpan.Zero)
            {
                httpResponse.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = options.HttpCacheMaxAge
                };
            }
        }


        private void AddHeaders(HttpResponse httpResponse, RendertronOptions options, bool isPrerendered) {
            if (options.AddVeryUserAgentHeader) {
                httpResponse.Headers.Append(HeaderNames.Vary, "User-Agent");
            }
            if (options.AddPrerenderedHeader) {
                httpResponse.Headers["Prerendered"] = new string[] { isPrerendered ? "1" : "0" };
            }
        }
    }
}
