//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidGuestId = Guid.Empty;
            var invalidGuestException = new InvalidGuestException();

            invalidGuestException.AddData(
                key: nameof(Guest.Id),
                values: "Id is required");

            var expectedGuestValidationException =
                new GuestValidationException(invalidGuestException);

            // when
            ValueTask<Guest> retrieveGuestByIdTask =
                this.guestService.RetrieveGuestByIdAsync(invalidGuestId);

            GuestValidationException actualGuestValidationException =
                await Assert.ThrowsAsync<GuestValidationException>(retrieveGuestByIdTask.AsTask);

            // then
            actualGuestValidationException.Should().BeEquivalentTo(expectedGuestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfGuestIsNotFoundAndLogItAsync()
        {
            // given
            Guid someGuestId = Guid.NewGuid();
            Guest noGuest = null;
            var notFoundGuestValidationException = new NotFoundGuestException(someGuestId);

            var expectedGuestValidationException =
                new GuestValidationException(notFoundGuestValidationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noGuest);

            // when
            ValueTask<Guest> retrieveGuestByIdTask =
                this.guestService.RetrieveGuestByIdAsync(someGuestId);

            GuestValidationException actualGuestValidationException =
                await Assert.ThrowsAsync<GuestValidationException>(retrieveGuestByIdTask.AsTask);

            // then
            actualGuestValidationException.Should().BeEquivalentTo(
                expectedGuestValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}