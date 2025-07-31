using FluentValidation;

namespace Manager.Domain.Requests.Cliente.Validators
{
    public class AddClienteRequestValidator : AbstractValidator<AddClienteRequest>
    {
        public AddClienteRequestValidator()
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