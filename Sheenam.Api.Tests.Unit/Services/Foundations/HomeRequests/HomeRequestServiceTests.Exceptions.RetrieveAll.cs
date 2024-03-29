﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlErrorOccursAndLogIt()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedHomeRequestStorageException =
                new FailedHomeRequestStorageException(sqlException);

            var excpectedHomeRequestDependencyException =
                new HomeRequestDependencyException(failedHomeRequestStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHomeRequests()).Throws(sqlException);

            // when
            Action retrieveAllHomeRequestAction = () =>
                this.homeRequestService.RetrieveAllHomeRequests();

            HomeRequestDependencyException actualHomeRequestDependencyException =
                Assert.Throws<HomeRequestDependencyException>(retrieveAllHomeRequestAction);

            // then
            actualHomeRequestDependencyException.Should().BeEquivalentTo(
                excpectedHomeRequestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHomeRequests(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    excpectedHomeRequestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogIt()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);

            var failedHomeRequestServiceException =
                new FailedHomeRequestServiceException(serviceException);

            var expectedHomeRequestServiceException =
                new HomeRequestServiceException(failedHomeRequestServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHomeRequests()).Throws(serviceException);

            // when
            Action retrieveHomeRequestAction = () =>
                this.homeRequestService.RetrieveAllHomeRequests();

            HomeRequestServiceException actualHomeRequestServiceException =
                Assert.Throws<HomeRequestServiceException>(retrieveHomeRequestAction);

            // then
            actualHomeRequestServiceException.Should().BeEquivalentTo(
                expectedHomeRequestServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHomeRequests(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}