using CasinoProject.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Xunit;

namespace CasinoProject.Infrastructure.Tests.Extensions
{
    public class InfrastructureExtensionsTests
    {
        private readonly IConfiguration Configuration;
        private readonly IServiceCollection ServiceCollection;
        public InfrastructureExtensionsTests()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("fakeAppsettings.json", optional: false, reloadOnChange: true);
            Configuration = configurationBuilder.Build();
            ServiceCollection = new ServiceCollection();
        }

        [Fact]
        public void InstallInfrastructure_ShouldNotThrowArgumentNullException()
        {
            // Act
            Func<IServiceCollection> act = () => ServiceCollection.InstallInfrastructure(Configuration);

            // Assert
            _ = act.Should().NotThrow();
        }

        [Fact]
        public void InstallInfrastructure_ShouldThrowArgumentNullException_WhenIServiceCollectionIsNull()
        {
            // Act
            Func<IServiceCollection> act = () => ((IServiceCollection)null).InstallInfrastructure(Configuration);


            // Assert
            _ = act.Should()
                   .ThrowExactly<ArgumentNullException>()
                   .WithMessage("IServiceCollection is null (Parameter 'services')");
        }

        [Fact]
        public void InstallInfrastructure_ShouldThrowArgumentNullException_WhenIConfigurationIsNull()
        {
            // Act
            Func<IServiceCollection> act = () => ServiceCollection.InstallInfrastructure(null);

            // Assert
            _ = act.Should()
                .ThrowExactly<ArgumentNullException>()
                .WithMessage("IConfiguration is null (Parameter 'configuration')");
        }
    }
}
