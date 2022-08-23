using MediatR;

namespace CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance
{
    public class GetCasinoBalanceQuery : IRequest<GetCasinoBalanceVM>
    {
    }

    public class GetCasinoBalanceVM
    {
        public decimal Balance { get; set; }
    }
}
