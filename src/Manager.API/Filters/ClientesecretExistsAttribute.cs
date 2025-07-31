using Manager.Domain.Requests.Cliente;
using Manager.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"Cliente with id {id} not exist." });
                    return;
                }

                await next();
            }
        }
    }
}