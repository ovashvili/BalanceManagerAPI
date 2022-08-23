using Balances;
using CasinoProject.Application.BalanceManager.Queries.Common.Models;
using MediatR;

namespace CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino
{
    public class WithdrawFromCasinoQuery : BaseQueryModel, IRequest<WithdrawFromCasinoVM>
    {
    }

    public class WithdrawFromCasinoVM
    {
        public ErrorCode ErrorCode { get; set; }
    }
}
