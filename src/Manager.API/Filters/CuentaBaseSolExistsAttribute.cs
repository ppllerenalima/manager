namespace Manager.API.Filters
{
    public class CuentaBaseSolExistsAttribute : TypeFilterAttribute
    {
        public CuentaBaseSolExistsAttribute() : base(typeof(CuentaBaseSolExistsFilterImpl)) { }

        public class CuentaBaseSolExistsFilterImpl : IAsyncActionFilter
        {
            private readonly ICuentaBaseSolService _grupoService;

            public CuentaBaseSolExistsFilterImpl(ICuentaBaseSolService grupoService)
            {
                _grupoService = grupoService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!(context.ActionArguments["id"] is Guid id))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                var result = await _grupoService.GetCuentaBaseSolAsync(new GetCuentaBaseSolRequest { Id = id });

                if (result == null)
                {
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"El grupo con id {id} no existe." });
                    return;
                }

                await next();
            }
        }
    }
}
