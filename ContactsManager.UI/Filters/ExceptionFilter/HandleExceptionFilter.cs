﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ExceptionFilter
{
    public class HandleExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {

            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError("Exception Filter {FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}",nameof(HandleExceptionFilter), nameof(OnExceptionAsync),context.Exception.GetType().ToString(),context.Exception.Message);

            if (_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult() { Content = context.Exception.Message };
            }
            return Task.CompletedTask;                
        }
    }
}
