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
        public async Task ShouldModifyHostAsync()
        {
            // given
            Host randomHost = CreateRandomHost();
            Host inputHost = randomHost;
            Host storageHost = inputHost;
            Host updatedHost = inputHost;
            Host expectedHost = updatedHost.DeepClone();
            Guid inputHostId = inputHost.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHostByIdAsync(inputHostId)).ReturnsAsync(storageHost);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateHostAsync(inputHost)).ReturnsAsync(updatedHost);

            // when
            Host actualHost =
                await this.hostService.ModifyHostAsync(inputHost);

            // then
            actualHost.Should().BeEquivalentTo(expectedHost);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHostByIdAsync(inputHostId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHostAsync(inputHost), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}