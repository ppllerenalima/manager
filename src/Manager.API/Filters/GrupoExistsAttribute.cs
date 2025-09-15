using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class GrupoExistsAttribute : TypeFilterAttribute
    {
        public GrupoExistsAttribute() : base(typeof(GrupoExistsFilterImpl)) { }

        public class GrupoExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IGrupoService _grupoService;

            public GrupoExistsFilterImpl(IGrupoService grupoService)
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

                var result = await _grupoService.GetGrupoAsync(new GetGrupoRequest { Id = id });

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
