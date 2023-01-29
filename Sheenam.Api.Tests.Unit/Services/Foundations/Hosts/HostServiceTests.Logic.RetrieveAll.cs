using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Hosts;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Hosts
{
    public partial class HostServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllHosts()
        {
            // given
            IQueryable<Host> randomHosts = CreateRandomHosts();
            IQueryable<Host> storageHosts = randomHosts;
            IQueryable<Host> expectedHosts = storageHosts;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHosts()).Returns(storageHosts);

            // when
            IQueryable<Host> actualHost = this.hostService.RetrieveAllHosts();

            // then
            actualHost.Should().BeEquivalentTo(expectedHosts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGuests(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}