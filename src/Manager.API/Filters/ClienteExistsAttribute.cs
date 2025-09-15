using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class ClienteExistsAttribute : TypeFilterAttribute
    {
        public ClienteExistsAttribute() : base(typeof(ClienteExistsFilterImpl)) { }

        public class ClienteExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IClienteService _clienteService;

            public ClienteExistsFilterImpl(IClienteService clienteService)
            {
                _clienteService = clienteService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!(context.ActionArguments["id"] is Guid id))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                var result = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = id });

                if (result == null)
                {
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"El cliente con id {id} no existe." });
                    return;
                }

                await next();
            }
        }
    }
}