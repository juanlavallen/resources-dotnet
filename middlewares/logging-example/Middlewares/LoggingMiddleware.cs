namespace Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Logging logic before the request is processed

            await _next(context);

            // Logging logic after the request has been processed
        }
    }
}
