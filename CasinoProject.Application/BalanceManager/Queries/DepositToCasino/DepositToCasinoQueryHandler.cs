using CasinoProject.Application.BalanceManager.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoProject.Application.BalanceManager.Queries.DepositToCasino
{
    public class DepositToCasinoQueryHandler : IRequestHandler<DepositToCasinoQuery, DepositToCasinoVM>
    {
        private readonly IBalanceManagerService _balanceManagerService;

        public DepositToCasinoQueryHandler(IBalanceManagerService balanceManagerService)
        {
            _balanceManagerService = balanceManagerService;
        }

        public async Task<DepositToCasinoVM> Handle(DepositToCasinoQuery request, CancellationToken cancellationToken)
        {
            return await _balanceManagerService.DepositToCasinoAsync(request);
        }
    }
}
