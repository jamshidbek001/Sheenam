//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public async Task ShouldRemoveHomeRequestByIdAsync()
        {
            // given
            Guid randomHomeRequestId = Guid.NewGuid();
            Guid inputHomeRequestId = Guid.NewGuid();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest();
            randomHomeRequest.Id = inputHomeRequestId;
            HomeRequest storageHomeRequest = randomHomeRequest;
            HomeRequest expectedHomeRequest = storageHomeRequest;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeRequestByIdAsync(inputHomeRequestId)).ReturnsAsync(expectedHomeRequest);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteHomeRequestAsync(storageHomeRequest)).ReturnsAsync(expectedHomeRequest);

            // when
            HomeRequest actualHomeRequest =
                await this.homeRequestService.RemoveHomeRequestByIdAsync(inputHomeRequestId);

            // then
            actualHomeRequest.Should().BeEquivalentTo(expectedHomeRequest);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(inputHomeRequestId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHomeRequestAsync(storageHomeRequest), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}