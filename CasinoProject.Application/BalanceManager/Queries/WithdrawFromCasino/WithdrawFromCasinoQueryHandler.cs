using CasinoProject.Application.BalanceManager.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino
{
    public class WithdrawFromCasinoQueryHandler : IRequestHandler<WithdrawFromCasinoQuery, WithdrawFromCasinoVM>
    {
        private readonly IBalanceManagerService _balanceManagerService;

        public WithdrawFromCasinoQueryHandler(IBalanceManagerService balanceManagerService)
        {
            _balanceManagerService = balanceManagerService;
        }

        public async Task<WithdrawFromCasinoVM> Handle(WithdrawFromCasinoQuery request, CancellationToken cancellationToken)
        {
            return await _balanceManagerService.WithdrawFromCasinoAsync(request);
        }
    }
}
