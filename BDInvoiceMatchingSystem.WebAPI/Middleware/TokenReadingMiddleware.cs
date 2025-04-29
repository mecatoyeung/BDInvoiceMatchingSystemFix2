namespace BDInvoiceMatchingSystem.WebAPI.Middleware
{
    public class TokenReadingMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenReadingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Headers["X-Access-Token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            await _next(context);
        }
    }
}
