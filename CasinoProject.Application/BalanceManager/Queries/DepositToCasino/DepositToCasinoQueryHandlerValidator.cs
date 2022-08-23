using FluentValidation;

namespace CasinoProject.Application.BalanceManager.Queries.DepositToCasino
{
    public class DepositToCasinoQueryHandlerValidator : AbstractValidator<DepositToCasinoQuery>
    {
        public DepositToCasinoQueryHandlerValidator()
        {
            _ = RuleFor(x => x.TransactionId)
                .NotNull()
                .NotEmpty()
                .MaximumLength(16);

            _ = RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
