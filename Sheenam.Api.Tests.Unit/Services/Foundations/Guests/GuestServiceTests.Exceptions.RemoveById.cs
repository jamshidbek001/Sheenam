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
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someGuestId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();
            var failedGuestStorageException = new FailedGuestStorageException(sqlException);

            var expectedGuestDependencyException =
                new GuestDependencyException(failedGuestStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>())).ThrowsAsync(sqlException);

            // when
            ValueTask<Guest> removeGuestTask =
                this.guestService.RemoveGuestByIdAsync(someGuestId);

            GuestDependencyException actualGuestDependencyException =
                await Assert.ThrowsAsync<GuestDependencyException>(removeGuestTask.AsTask);

            // then
            actualGuestDependencyException.Should().BeEquivalentTo(
                expectedGuestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGuestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someGuestId = Guid.NewGuid();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedGuestException = new LockedGuestException(databaseUpdateConcurrencyException);

            var expectedGuestDependencyValidationException =
                new GuestDependencyValidationException(lockedGuestException);

            this.storageBrokerMock.Setup(broker => broker.SelectGuestByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Guest> removeGuestTask =
                this.guestService.RemoveGuestByIdAsync(someGuestId);

            GuestDependencyValidationException actualGuestDependencyValidationException =
                await Assert.ThrowsAsync<GuestDependencyValidationException>(removeGuestTask.AsTask);

            // then
            actualGuestDependencyValidationException.Should().BeEquivalentTo(
                expectedGuestDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGuestAsync(It.IsAny<Guest>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someGuestId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedGuestServiceException =
                new FailedGuestServiceException(serviceException);

            var expectedGuestServiceException =
                new GuestServieException(failedGuestServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            // when
            ValueTask<Guest> removeGuestTask =
                this.guestService.RemoveGuestByIdAsync(someGuestId);

            GuestServieException actualGuestServieException =
                await Assert.ThrowsAsync<GuestServieException>(removeGuestTask.AsTask);

            // then
            actualGuestServieException.Should().BeEquivalentTo(expectedGuestServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGuestByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGuestServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}