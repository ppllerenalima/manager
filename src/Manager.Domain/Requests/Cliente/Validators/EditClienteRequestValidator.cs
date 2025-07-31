using FluentValidation;

namespace Manager.Domain.Requests.Cliente.Validators
{
    public class EditClienteRequestValidator : AbstractValidator<EditClienteRequest>
    {
        public EditClienteRequestValidator()
        {
            RuleFor(x => x.Ruc).NotEmpty();
            RuleFor(x => x.Razonsocial).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}