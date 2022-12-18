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
        public async Task ShouldAddHomeAsync()
        {
            // given
            Home randomHome = CreateRandomHome();
            Home inputHome = randomHome;
            Home storageHome = inputHome;
            Home expectedHome = storageHome.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHomeAsync(inputHome))
                    .ReturnsAsync(storageHome);

            // when
            Home actualHome =
                await this.homeService.AddHomeAsync(inputHome);

            // then
            actualHome.Should().BeEquivalentTo(expectedHome);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeAsync(inputHome), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}