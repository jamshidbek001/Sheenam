//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guest someGuest = CreateRandomGuest();
            SqlException sqlException = GetSqlError();
            var failedGuestStorageException = new FailedGuestStorageException(sqlException);

            var expectedGuestDependencyException =
                new GuestDependencyException(failedGuestStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(someGuest.Id)).ThrowsAsync(sqlException);

            // when
            ValueTask<Guest> modifyGuestTask =
                this.guestService.ModifyGuestAsync(someGuest);

            GuestDependencyException actualGuestDependencyException =
                await Assert.ThrowsAsync<GuestDependencyException>(modifyGuestTask.AsTask);

            // then
            actualGuestDependencyException.Should().BeEquivalentTo(expectedGuestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(someGuest.Id), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGuestAsync(It.IsAny<Guest>()), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGuestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guest randomGuest = CreateRandomGuest(randomDateTime);
            Guest someGuest = randomGuest;
            Guid guestId = someGuest.Id;
            var databaseUpdateException = new DbUpdateException();
            var failedGuestException = new FailedGuestStorageException(databaseUpdateException);

            var expectedGuestDependencyException =
                new GuestDependencyException(failedGuestException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(guestId)).ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<Guest> modifyGuestTAsk =
                this.guestService.ModifyGuestAsync(someGuest);

            GuestDependencyException actualGuestDependencyException =
                await Assert.ThrowsAsync<GuestDependencyException>(modifyGuestTAsk.AsTask);

            // then
            actualGuestDependencyException.Should().BeEquivalentTo(expectedGuestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(guestId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guest randomGuest = CreateRandomGuest(randomDateTime);
            Guest someGuest = randomGuest;
            Guid guestId = someGuest.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedGuestException = new LockedGuestException(databaseUpdateConcurrencyException);

            var expectedGuestDependencyValidationException =
                new GuestDependencyValidationException(lockedGuestException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(guestId)).ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Guest> modifyGuestTask =
                this.guestService.ModifyGuestAsync(someGuest);

            GuestDependencyValidationException actualGuestDependencyValidationException =
                await Assert.ThrowsAsync<GuestDependencyValidationException>(modifyGuestTask.AsTask);

            // then
            actualGuestDependencyValidationException.Should().BeEquivalentTo(
                expectedGuestDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(guestId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guest randomGuest = CreateRandomGuest(randomDateTime);
            Guest someGuest = randomGuest;
            var serviceException = new Exception();
            var failedGuestException = new FailedGuestServiceException(serviceException);
            var expectedGuestServiceExeption = new GuestServieException(failedGuestException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(someGuest.Id)).ThrowsAsync(serviceException);

            // when
            ValueTask<Guest> modifyGustTask =
                this.guestService.ModifyGuestAsync(someGuest);

            GuestServieException actualGuestServieException =
                await Assert.ThrowsAsync<GuestServieException>(modifyGustTask.AsTask);

            // then
            actualGuestServieException.Should().BeEquivalentTo(expectedGuestServiceExeption);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(someGuest.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestServiceExeption))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}