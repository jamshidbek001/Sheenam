//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldRemoveGuestByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputGuestId = randomId;
            Guest randomGuest = CreateRandomGuest();
            Guest storageGuest = randomGuest;
            Guest expectedInputGuest = storageGuest;
            Guest deletedGuest = expectedInputGuest;
            Guest expectedGuest = deletedGuest.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(inputGuestId)).ReturnsAsync(storageGuest);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteGuestAsync(expectedInputGuest)).ReturnsAsync(deletedGuest);

            // when
            Guest actualGuest =
                await this.guestService.RemoveGuestByIdAsync(inputGuestId);

            // then
            actualGuest.Should().BeEquivalentTo(expectedGuest);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(inputGuestId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGuestAsync(expectedInputGuest), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}