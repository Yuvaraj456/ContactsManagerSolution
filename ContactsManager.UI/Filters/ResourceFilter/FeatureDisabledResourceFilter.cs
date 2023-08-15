using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourceFilter
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;

        private readonly bool _isDisabled;

        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger, bool disabled = true)
        {
            _logger = logger;
            _isDisabled = disabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //To Do Before Logic
            _logger.LogInformation("{filtername}.{methodname} - before", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
            
            if(_isDisabled)
            {
                //context.Result = new NotFoundResult(); //404 NotFound for permanent disable

                context.Result = new StatusCodeResult(501); //501 Not implemented for temporary disable
            }
            else
            {
                await next();

            }
           

            //To DO After Logic
            _logger.LogInformation("{filtername}.{methodname} - after", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));


        }
    }
}
