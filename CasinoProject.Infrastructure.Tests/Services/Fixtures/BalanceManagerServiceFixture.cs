using Balances;
using CasinoProject.Infrastructure.Enumerations.BalanceManager;
using CasinoProject.Infrastructure.Options;
using CasinoProject.Infrastructure.Services.BalanceManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;

namespace CasinoProject.Infrastructure.Tests.Services.Fixtures
{
    public class BalanceManagerServiceFixture
    {
        public BalanceManagerService BalanceManagerService => new BalanceManagerService(ServiceResolver, RetryCountOptionMock.Object, LoggerMock.Object);
        public Mock<IBalanceManager> CasinoBalanceManagerMock { get; set; }
        public Mock<IBalanceManager> GameBalanceManagerMock { get; set; }
        public Mock<IOptions<RetryCountOption>> RetryCountOptionMock { get; set; }
        public Func<BalanceManagerType, IBalanceManager> ServiceResolver { get; }
        public Mock<ILogger<BalanceManagerService>> LoggerMock { get; set; }
        public IServiceCollection ServiceCollection { get; }
        public IServiceProvider ServiceProvider { get; }

        public BalanceManagerServiceFixture()
        {
            ServiceCollection = new ServiceCollection();

            ServiceProvider = ServiceCollection.BuildServiceProvider();

            RetryCountOptionMock = new Mock<IOptions<RetryCountOption>>();

            ServiceResolver = (BalanceManagerType key) => key == BalanceManagerType.Game ? GameBalanceManagerMock.Object : CasinoBalanceManagerMock.Object;

            GameBalanceManagerMock = new Mock<IBalanceManager>(MockBehavior.Loose);

            CasinoBalanceManagerMock = new Mock<IBalanceManager>(MockBehavior.Loose);

            LoggerMock = new Mock<ILogger<BalanceManagerService>>();

            _ = RetryCountOptionMock
                .Setup(x => x.Value)
                .Returns(new RetryCountOption
                {
                    Withdraw = 3,
                    Deposit = 3,
                    Rollback = 3,
                });

            _ = CasinoBalanceManagerMock.Setup(x => x.GetBalance())
                .Returns(9999m);

            _ = GameBalanceManagerMock.Setup(x => x.GetBalance())
                .Returns(9999m);

            _ = GameBalanceManagerMock.Setup(x => x.CheckTransaction("12"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = CasinoBalanceManagerMock.Setup(x => x.CheckTransaction("12"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = CasinoBalanceManagerMock.Setup(x => x.Rollback("12"))
                .Returns(ErrorCode.TransactionRollbacked);

            _ = CasinoBalanceManagerMock.Setup(x => x.CheckTransaction("101"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = CasinoBalanceManagerMock.Setup(x => x.CheckTransaction("55"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = GameBalanceManagerMock.Setup(x => x.CheckTransaction("55"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = GameBalanceManagerMock.Setup(x => x.Rollback("55"))
                .Returns(ErrorCode.TransactionRollbacked);

            _ = GameBalanceManagerMock.Setup(x => x.CheckTransaction("721"))
                .Returns(ErrorCode.TransactionNotFound);

            _ = ServiceCollection.AddScoped(_ => CasinoBalanceManagerMock.Object);
            _ = ServiceCollection.AddScoped(_ => GameBalanceManagerMock.Object);
            _ = ServiceCollection.AddSingleton(_ => RetryCountOptionMock.Object);
        }
    }
}
