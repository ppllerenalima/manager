using Manager.Domain.Requests.Role;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Filters
{
    public class RoleExistsAttribute : TypeFilterAttribute
    {
        public RoleExistsAttribute() : base(typeof(RoleExistsFilterImpl)) { }

        public class RoleExistsFilterImpl : IAsyncActionFilter
        {
            private readonly IRoleService _roleService;

            public RoleExistsFilterImpl(IRoleService roleService)
            {
                _roleService = roleService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!(context.ActionArguments["id"] is Guid id))
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                var result = await _roleService.GetRoleAsync(new GetRoleRequest { Id = id });

                if (result == null)
                {
                    context.Result = new NotFoundObjectResult(new JsonErrorPayload { DetailedMessage = $"El role con id {id} no existe." });
                    return;
                }

                await next();
            }
        }
    }
}
