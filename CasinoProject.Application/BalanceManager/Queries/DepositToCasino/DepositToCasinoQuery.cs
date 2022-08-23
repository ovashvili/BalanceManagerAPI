using Balances;
using CasinoProject.Application.BalanceManager.Queries.Common.Models;
using MediatR;

namespace CasinoProject.Application.BalanceManager.Queries.DepositToCasino
{
    public class DepositToCasinoQuery : BaseQueryModel, IRequest<DepositToCasinoVM>
    {
    }

    public class DepositToCasinoVM
    {
        public ErrorCode ErrorCode { get; set; }
    }
}
