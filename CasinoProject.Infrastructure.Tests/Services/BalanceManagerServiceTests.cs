using Balances;
using CasinoProject.Application.BalanceManager.Queries.DepositToCasino;
using CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino;
using CasinoProject.Infrastructure.Tests.Services.Fixtures;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace CasinoProject.Infrastructure.Tests.Services
{
    public class BalanceManagerServiceTests : IClassFixture<BalanceManagerServiceFixture>
    {
        private readonly BalanceManagerServiceFixture _fixture;

        public BalanceManagerServiceTests(BalanceManagerServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetCasinoBalanceAsync_ShouldReturnCasinoBalance()
        {
            // Arrange
            var sut = _fixture;
            
            // Act
            var response = await sut.BalanceManagerService.GetCasinoBalanceAsync();

            // Assert
            _ = response.Balance.Should().Be(9999m);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnSuccess_WhenTransactionIsSuccessfull()
        {

            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(1010m, "12"))
                .Returns(ErrorCode.Success);
            
            sut.CasinoBalanceManagerMock.Setup(x => x.IncreaseBalance(1010m, "12"))
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "12",
                Amount = 1010m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnNotEnoughBalance()
        {

            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(10101m, "12"))
                .Returns(ErrorCode.NotEnoughtBalance);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "12",
                Amount = 10101m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.NotEnoughtBalance);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnTransactionNotFound_WhenDecreaseBalanceReturnsUnknownError()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(1010m, "12"))
                .Returns(ErrorCode.UnknownError);
            
            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "12",
                Amount = 1010m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.TransactionNotFound);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnSuccess_WhenDecreaseBalanceReturnsUnknownError()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(5001m, "101"))
                .Returns(ErrorCode.UnknownError);

            sut.CasinoBalanceManagerMock.Setup(x => x.IncreaseBalance(5001m, "101"))
                .Returns(ErrorCode.Success);

            sut.GameBalanceManagerMock.SetupSequence(x => x.CheckTransaction("101"))
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "101",
                Amount = 5001m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnSuccess_WhenDecreaseBalanceReturnsUnknownError_RetryCount2()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(505m, "101"))
                .Returns(ErrorCode.UnknownError);

            sut.CasinoBalanceManagerMock.Setup(x => x.IncreaseBalance(505m, "101"))
                .Returns(ErrorCode.Success);

            sut.GameBalanceManagerMock.SetupSequence(x => x.CheckTransaction("101"))
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "101",
                Amount = 505m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldReturnDuplicateTransactionId_WhenTransactionAlreadyExists()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.CheckTransaction("25"))
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "25",
                Amount = 97m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.DuplicateTransactionId);
        }

        [Fact]
        public async Task DepositToCasinoAsync_ShouldRollbackTransaction_AfterThreeIneffectiveDepositRetries()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.DecreaseBalance(751m, "12"))
                .Returns(ErrorCode.Success);

            sut.CasinoBalanceManagerMock.Setup(x => x.IncreaseBalance(751m, "12"))
                .Returns(ErrorCode.UnknownError);

            sut.GameBalanceManagerMock.Setup(x => x.Rollback("12"))
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "12",
                Amount = 751m
            });

            // Assert
            sut.GameBalanceManagerMock.Verify(x => x.Rollback("12"));
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnSuccess_WhenTransactionIsSuccessfull()
        {
            // Arrange
            var sut = _fixture;

            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(2452m, "55"))
                .Returns(ErrorCode.Success);

            sut.GameBalanceManagerMock.Setup(x => x.IncreaseBalance(2452m, "55"))
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "55",
                Amount = 2452m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnNotEnoughBalance()
        {

            // Arrange
            var sut = _fixture;

            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(20202m, "55"))
                .Returns(ErrorCode.NotEnoughtBalance);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "55",
                Amount = 20202m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.NotEnoughtBalance);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnTransactionNotFound_WhenDecreaseBalanceReturnsUnknownError()
        {
            // Arrange
            var sut = _fixture;

            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(2070m, "55"))
                .Returns(ErrorCode.UnknownError);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "55",
                Amount = 2070m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.TransactionNotFound);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnSuccess_WhenDecreaseBalanceReturnsUnknownError()
        {
            // Arrange
            var sut = _fixture;

            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(3001m, "721"))
                .Returns(ErrorCode.UnknownError);

            sut.GameBalanceManagerMock.Setup(x => x.IncreaseBalance(3001m, "721"))
                .Returns(ErrorCode.Success);

            sut.CasinoBalanceManagerMock.SetupSequence(x => x.CheckTransaction("721"))
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "721",
                Amount = 3001m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnSuccess_WhenDecreaseBalanceReturnsUnknownError_RetryCount3()
        {
            // Arrange
            var sut = _fixture;
            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(805m, "721"))
                .Returns(ErrorCode.UnknownError);

            sut.GameBalanceManagerMock.Setup(x => x.IncreaseBalance(805m, "721"))
                .Returns(ErrorCode.Success);

            sut.CasinoBalanceManagerMock.SetupSequence(x => x.CheckTransaction("721"))
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.TransactionNotFound)
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "721",
                Amount = 805m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.Success);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldReturnDuplicateTransactionId_WhenTransactionAlreadyExists()
        {
            // Arrange
            var sut = _fixture;

            sut.GameBalanceManagerMock.Setup(x => x.CheckTransaction("825"))
                .Returns(ErrorCode.TransactionRollbacked);

            // Act
            var response = await sut.BalanceManagerService.DepositToCasinoAsync(new DepositToCasinoQuery
            {
                TransactionId = "825",
                Amount = 122m
            });

            // Assert
            _ = response.ErrorCode.Should().Be(ErrorCode.DuplicateTransactionId);
        }

        [Fact]
        public async Task WithdrawFromCasinoAsync_ShouldRollbackTransaction_AfterThreeIneffectiveDepositRetries()
        {
            // Arrange
            var sut = _fixture;

            sut.CasinoBalanceManagerMock.Setup(x => x.DecreaseBalance(636m, "55"))
                .Returns(ErrorCode.Success);

            sut.GameBalanceManagerMock.Setup(x => x.IncreaseBalance(636m, "55"))
                .Returns(ErrorCode.UnknownError);

            sut.CasinoBalanceManagerMock.Setup(x => x.Rollback("55"))
                .Returns(ErrorCode.Success);

            // Act
            var response = await sut.BalanceManagerService.WithdrawFromCasinoAsync(new WithdrawFromCasinoQuery
            {
                TransactionId = "55",
                Amount = 636m
            });

            // Assert
            sut.CasinoBalanceManagerMock.Verify(x => x.Rollback("55"));
        }
    }
}
