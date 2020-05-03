using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;

namespace Shared
{
    public sealed class TestDiagnosticListener
    {
        private readonly string _path;
        private readonly ILogger<TestDiagnosticListener> _logger;

        public TestDiagnosticListener(string path, ILoggerFactory loggerFactory)
        {
            _path = path;
            _logger = loggerFactory.CreateLogger<TestDiagnosticListener>();
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
        public void OnMiddlewareStarting(HttpContext httpContext, string name)
        {
            //if (httpContext.Request.Path != _path) return;
            _logger.LogInformation(
                "Middleware {name} started: Scheme: {protocol}, Path: {path}, PathBase: {pathBase}",
                name,
                httpContext.Request.Scheme,
                httpContext.Request.Path,
                httpContext.Request.PathBase);
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
        public void OnMiddlewareException(Exception exception, string name)
        {
            _logger.LogInformation("Middleware {name} exception: {exception}", name, exception.Message);
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
        public void OnMiddlewareFinished(HttpContext httpContext, string name)
        {
            //if (httpContext.Request.Path != _path) return;
            _logger.LogInformation(
                "Middleware {name} finished: Scheme: {protocol}, Path: {path}, PathBase: {pathBase}",
                name,
                httpContext.Request.Scheme,
                httpContext.Request.Path,
                httpContext.Request.PathBase);
        }
    }
}