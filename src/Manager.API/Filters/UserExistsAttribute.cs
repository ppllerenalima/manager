using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class UserExistsAttribute : TypeFilterAttribute
    {
        public UserExistsAttribute() : base(typeof(UserExistsFilterImpl)) { }

        public class UserExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IUserService _userService;

            public UserExistsFilterImpl(IUserService userService)
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

                var result = await _userService.GetUserAsync(new GetUserRequest { Id = id });

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
