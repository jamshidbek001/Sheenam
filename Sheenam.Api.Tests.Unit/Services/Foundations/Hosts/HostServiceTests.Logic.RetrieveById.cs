//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.Hosts;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Hosts
{
    public partial class HostServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveHostByIdAsync()
        {
            // given
            Guid randomHostId = Guid.NewGuid();
            Guid inputHostId = randomHostId;
            Host randomHost = CreateRandomHost();
            Host storageHost = randomHost;
            Host expectedHost = storageHost.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHostByIdAsync(inputHostId)).ReturnsAsync(storageHost);

            // when
            Host actualHost = await this.hostService.RetrieveHostByIdAsync(inputHostId);

            // then
            actualHost.Should().BeEquivalentTo(expectedHost);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHostByIdAsync(inputHostId), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}