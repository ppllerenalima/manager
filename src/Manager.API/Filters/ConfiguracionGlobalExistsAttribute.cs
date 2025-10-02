using Manager.Domain.Requests.ConfiguracionGlobal;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class ConfiguracionGlobalExistsAttribute : TypeFilterAttribute
    {
        public ConfiguracionGlobalExistsAttribute() : base(typeof(ConfiguracionGlobalExistsFilterImpl)) { }

        public class ConfiguracionGlobalExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IConfiguracionGlobalService _ConfiguracionGlobalService;

            public ConfiguracionGlobalExistsFilterImpl(IConfiguracionGlobalService ConfiguracionGlobalService)
            {
                _ConfiguracionGlobalService = ConfiguracionGlobalService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!(context.ActionArguments["id"] is Guid id))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                var result = await _ConfiguracionGlobalService.GetConfiguracionGlobalAsync(new GetConfiguracionGlobalRequest { Id = id });

                if (result == null)
                {
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"El ConfiguracionGlobal con id {id} no existe." });
                    return;
                }

                await next();
            }
        }
    }
}
