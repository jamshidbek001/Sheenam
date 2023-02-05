//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.Homes;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Homes
{
    public partial class HomeServiceTests
    {
        [Fact]
        public async Task ShouldRemoveHomeByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputHomeId = randomId;
            Home randomHome = CreateRandomHome();
            Home storageHome = randomHome;
            Home expectedInputHome = storageHome;
            Home deletedHome = expectedInputHome;
            Home expectedHome = deletedHome.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(inputHomeId)).ReturnsAsync(storageHome);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteHomeAsync(expectedInputHome)).ReturnsAsync(deletedHome);

            // when
            Home actualHome =
                await this.homeService.RemoveHomeByIdAsync(inputHomeId);

            // then
            actualHome.Should().BeEquivalentTo(expectedHome);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(inputHomeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHomeAsync(expectedInputHome), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}