using Balances;
using CasinoProject.Application.BalanceManager.Contracts;
using CasinoProject.Application.BalanceManager.Queries.Common.Models;
using CasinoProject.Application.BalanceManager.Queries.DepositToCasino;
using CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance;
using CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino;
using CasinoProject.Infrastructure.Options;
using CasinoProject.Infrastructure.Enumerations.BalanceManager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using CasinoProject.Infrastructure.Extensions;

namespace CasinoProject.Infrastructure.Services.BalanceManager
{
    public class BalanceManagerService : IBalanceManagerService
    {
        private readonly IBalanceManager gameBalanceManager;
        private readonly IBalanceManager casinoBalanceManager;
        private readonly RetryCountOption _retryCountOption;
        private readonly ILogger<BalanceManagerService> _logger;

        public BalanceManagerService(Func<BalanceManagerType, IBalanceManager> balanceManagerServiceResolver, IOptions<RetryCountOption> retryCountOpetion, ILogger<BalanceManagerService> logger)
        {
            gameBalanceManager = balanceManagerServiceResolver(BalanceManagerType.Game);
            casinoBalanceManager = balanceManagerServiceResolver(BalanceManagerType.Casino);
            _retryCountOption = retryCountOpetion.Value;
            _logger = logger;
        }

        public Task<GetCasinoBalanceVM> GetCasinoBalanceAsync()
        {
            return Task.FromResult(new GetCasinoBalanceVM
            {
                Balance = casinoBalanceManager.GetBalance()
            });
        }

        public Task<DepositToCasinoVM> DepositToCasinoAsync(DepositToCasinoQuery query)
        {
            var serviceResponse = new DepositToCasinoVM { ErrorCode = DecreaseBalance(gameBalanceManager, query, _retryCountOption.Withdraw) };
            _logger.LogBMInfo(nameof(DepositToCasinoAsync), "DecreaseGameBalanceStatus", serviceResponse.ErrorCode, query);

            if (serviceResponse.ErrorCode == ErrorCode.Success)
            {
                serviceResponse.ErrorCode = IncreaseBalance(casinoBalanceManager, query, _retryCountOption.Deposit);
                _logger.LogBMInfo(nameof(DepositToCasinoAsync), "IncreaseCasinoBalanceStatus", serviceResponse.ErrorCode, query);
                
                if (serviceResponse.ErrorCode != ErrorCode.Success)
                {
                    var rollBackStatus = Rollback(gameBalanceManager, query.TransactionId, _retryCountOption.Rollback);
                    _logger.LogBMInfo(nameof(DepositToCasinoAsync), "RollbackGameBalanceStatus", rollBackStatus, query);
                }
            }

            return Task.FromResult(serviceResponse);
        }

        public Task<WithdrawFromCasinoVM> WithdrawFromCasinoAsync(WithdrawFromCasinoQuery query)
        {
            var serviceResponse = new WithdrawFromCasinoVM { ErrorCode = DecreaseBalance(casinoBalanceManager, query, _retryCountOption.Withdraw) };
            _logger.LogBMInfo(nameof(WithdrawFromCasinoAsync), "DecreaseCasinoBalanceStatus", serviceResponse.ErrorCode, query);
            
            if (serviceResponse.ErrorCode == ErrorCode.Success)
            {
                serviceResponse.ErrorCode = IncreaseBalance(gameBalanceManager, query, _retryCountOption.Deposit);
                _logger.LogBMInfo(nameof(WithdrawFromCasinoAsync), "IncreaseGameBalanceStatus", serviceResponse.ErrorCode, query);

                if (serviceResponse.ErrorCode != ErrorCode.Success)
                {
                    var rollBackStatus = Rollback(casinoBalanceManager, query.TransactionId, _retryCountOption.Rollback);
                    _logger.LogBMInfo(nameof(WithdrawFromCasinoAsync), "RollbackCasinoBalanceStatus", rollBackStatus, query);
                }
            }

            return Task.FromResult(serviceResponse);
        }

        private ErrorCode Rollback(IBalanceManager balanceManager, string transactionId, int retryCount)
        {
            var errorCode = ErrorCode.Success;

            while (retryCount-- > 0)
            {
                errorCode = balanceManager.Rollback(transactionId);

                if (errorCode == ErrorCode.Success)
                    return ErrorCode.TransactionRollbacked;

                if(errorCode == ErrorCode.UnknownError)
                    errorCode = balanceManager.CheckTransaction(transactionId);

                if (errorCode == ErrorCode.TransactionRollbacked)
                    break;
            }

            return errorCode;
        }

        private ErrorCode IncreaseBalance(IBalanceManager balanceManager, BaseQueryModel query, int retryCount)
        {
            var errorCode = balanceManager.CheckTransaction(query.TransactionId);

            if (errorCode != ErrorCode.TransactionNotFound)
                return ErrorCode.DuplicateTransactionId;

            while (retryCount-- > 0)
            {
                errorCode = balanceManager.IncreaseBalance(query.Amount, query.TransactionId);

                if (errorCode == ErrorCode.UnknownError)
                    errorCode = balanceManager.CheckTransaction(query.TransactionId);

                if (errorCode != ErrorCode.TransactionNotFound)
                    break;
            }

            return errorCode;
        }

        private ErrorCode DecreaseBalance(IBalanceManager balanceManager, BaseQueryModel query, int retryCount)
        {
            var errorCode = balanceManager.CheckTransaction(query.TransactionId);

            if (errorCode != ErrorCode.TransactionNotFound)
                return ErrorCode.DuplicateTransactionId;

            while (retryCount-- > 0)
            {
                errorCode = balanceManager.DecreaseBalance(query.Amount, query.TransactionId);

                if (errorCode == ErrorCode.UnknownError)
                    errorCode = balanceManager.CheckTransaction(query.TransactionId);

                if (errorCode != ErrorCode.TransactionNotFound)
                    break;
            }

            return errorCode;
        }
    }
}
