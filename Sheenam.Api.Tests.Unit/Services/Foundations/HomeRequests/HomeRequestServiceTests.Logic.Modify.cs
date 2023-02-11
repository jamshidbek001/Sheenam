//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public async Task ShouldModifyHomeRequestAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomModifyHomeRequest(randomDate);
            HomeRequest inputHomeRequest = randomHomeRequest;
            HomeRequest storageHomeRequest = inputHomeRequest.DeepClone();
            storageHomeRequest.UpdatedDate = randomHomeRequest.CreatedDate;
            HomeRequest updatedHomeRequest = inputHomeRequest;
            HomeRequest expectedHomeRequest = updatedHomeRequest.DeepClone();
            Guid homeRequestId = inputHomeRequest.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeRequestByIdAsync(homeRequestId)).ReturnsAsync(storageHomeRequest);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateHomeRequestAsync(inputHomeRequest)).ReturnsAsync(updatedHomeRequest);

            // when
            HomeRequest actualHomeRequest =
                await this.homeRequestService.ModifyHomeRequestAsync(inputHomeRequest);

            // then
            actualHomeRequest.Should().BeEquivalentTo(expectedHomeRequest);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(homeRequestId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHomeRequestAsync(inputHomeRequest), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}