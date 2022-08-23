using Balances;
using CasinoProject.Application.BalanceManager.Queries.Common.Models;
using Microsoft.Extensions.Logging;

namespace CasinoProject.Infrastructure.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogBMInfo(this ILogger logger, string method, string serviceCall, ErrorCode status, BaseQueryModel query)
        {
            logger.LogInformation($"{method}|{serviceCall}({status}); TransactionId({query.TransactionId}); Amount({query.Amount})");
        }
    }
}
