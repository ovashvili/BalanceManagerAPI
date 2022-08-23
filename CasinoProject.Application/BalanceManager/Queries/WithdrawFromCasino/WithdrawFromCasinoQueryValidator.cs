using FluentValidation;

namespace CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino
{
    public class WithdrawFromCasinoQueryValidator : AbstractValidator<WithdrawFromCasinoQuery>
    {
        public WithdrawFromCasinoQueryValidator()
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
