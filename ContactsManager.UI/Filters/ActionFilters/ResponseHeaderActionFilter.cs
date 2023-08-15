using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public int _Order { get; set; }

        private readonly string _key;
        private readonly string _value;

        public ResponseHeaderFilterFactoryAttribute(string key, string value,int Order)
        {
            _key = key;
            _value = value;
            _Order = Order;
            
        }
        //COntroller invokes filter factory, and filter factory invoke filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //return filter object 
            ResponseHeaderActionFilter filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>() ;
            filter.Order = _Order;
            filter._key = _key;
            filter._value = _value;
            return filter;

        }

    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
    {



        public int Order { get; set; }

        public  string _key { get; set; }
        public  string _value { get; set; }

        public readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger )
        {
          
           _logger = logger;
        }

        

        public  async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before
            _logger.LogInformation("Before Logic of ResponseHeaderActionFilter");

            await next(); //Calls subsequent filter or action method !important to add this line

            //after
            _logger.LogInformation("After Logic of ResponseHeaderActionFilter");

            context.HttpContext.Response.Headers[_key] = _value;

        }
       
    }
}
