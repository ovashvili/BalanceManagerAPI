namespace CasinoProject.Infrastructure.Options
{
    public class RetryCountOption
    {
        public int Withdraw { get; set; }
        public int Deposit { get; set; }
        public int Rollback { get; set; }
    }
}
