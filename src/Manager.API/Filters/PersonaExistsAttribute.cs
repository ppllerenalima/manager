using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class PersonaExistsAttribute : TypeFilterAttribute
    {
        public PersonaExistsAttribute() : base(typeof(PersonaExistsFilterImpl)) { }

        public class PersonaExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IPersonaService _userService;

            public PersonaExistsFilterImpl(IPersonaService userService)
            {
                _userService = userService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!(context.ActionArguments["id"] is Guid id))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                var result = await _userService.GetPersonaAsync(new GetPersonaRequest { Id = id });

                if (result == null)
                {
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"El user con id {id} no existe." });
                    return;
                }

                await next();
            }
        }
    }
}
