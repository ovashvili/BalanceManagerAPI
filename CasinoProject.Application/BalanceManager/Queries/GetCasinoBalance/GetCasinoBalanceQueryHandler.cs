using CasinoProject.Application.BalanceManager.Contracts;
using CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoProject.Application.BalanceManager.Queries.GetBalance
{
    public class GetCasinoBalanceQueryHandler : IRequestHandler<GetCasinoBalanceQuery, GetCasinoBalanceVM>
    {
        private readonly IBalanceManagerService _balanceManagerService;

        public GetCasinoBalanceQueryHandler(IBalanceManagerService balanceManagerService)
        {
            _balanceManagerService = balanceManagerService;
        }

        public async Task<GetCasinoBalanceVM> Handle(GetCasinoBalanceQuery request, CancellationToken cancellationToken)
        {
            return await _balanceManagerService.GetCasinoBalanceAsync();
        }
    }
}
