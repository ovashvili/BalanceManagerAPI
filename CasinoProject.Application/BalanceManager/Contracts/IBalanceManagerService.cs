using CasinoProject.Application.BalanceManager.Queries.DepositToCasino;
using CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance;
using CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino;
using System.Threading.Tasks;

namespace CasinoProject.Application.BalanceManager.Contracts
{
    public interface IBalanceManagerService
    {
        Task<GetCasinoBalanceVM> GetCasinoBalanceAsync();
        Task<DepositToCasinoVM> DepositToCasinoAsync(DepositToCasinoQuery query);
        Task<WithdrawFromCasinoVM> WithdrawFromCasinoAsync(WithdrawFromCasinoQuery query);
    }
}
