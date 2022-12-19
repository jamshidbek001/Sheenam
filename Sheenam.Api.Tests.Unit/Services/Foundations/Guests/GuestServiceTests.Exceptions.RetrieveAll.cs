//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.Guests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlErrorOccursAbdLogIt()
        {
            // given
            SqlException sqlException = GetSqlError();
            var failedGuestServiceException = new FailedGuestServiceException(sqlException);

            var expectedGuestDependencyException =
                new GuestDependencyException(failedGuestServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGuests()).Throws(sqlException);

            // when
            Action retrieveAllGuestAction = () =>
                this.guestService.RetrieveAllGuests();

            GuestDependencyException actualGuestDependencyException =
                Assert.Throws<GuestDependencyException>(retrieveAllGuestAction);

            // then
            actualGuestDependencyException.Should().BeEquivalentTo(expectedGuestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGuests(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGuestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenAllServiceErrorOccursAndLogIt()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);
            var failedGuestServiceException = new FailedGuestServiceException(serviceException);

            var expectedGuestServiceException =
                new GuestServieException(failedGuestServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGuests()).Throws(serviceException);

            // when
            Action retrieveAllGuestAction = () =>
                this.guestService.RetrieveAllGuests();

            GuestServieException actualGuestServieException =
                Assert.Throws<GuestServieException>(retrieveAllGuestAction);

            // then
            actualGuestServieException.Should().BeEquivalentTo(expectedGuestServiceException);
        }
    }
}