//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Data;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            HomeRequest someHomeRequest = CreateRandomHomeRequest();
            SqlException sqlException = CreateSqlException();

            var failedHomeRequestStorageException =
                new FailedHomeRequestStorageException(sqlException);

            var expectedHomeRequestDependencyException =
                new HomeRequestDependencyException(failedHomeRequestStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>())).ThrowsAsync(sqlException);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(someHomeRequest);

            HomeRequestDependencyException actualHomeRequestDependencyException =
                await Assert.ThrowsAsync<HomeRequestDependencyException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestDependencyException.Should().BeEquivalentTo(
                expectedHomeRequestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHomeRequestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            HomeRequest someHomeRequest = CreateRandomHomeRequest();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistHomeRequestException =
                new AlreadyExistHomeRequestException(duplicateKeyException);

            var expectedHomeRequestDependencyValidationException =
                new HomeRequestDependencyValidationException(alreadyExistHomeRequestException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>())).ThrowsAsync(duplicateKeyException);
            ;
            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(someHomeRequest);

            HomeRequestDependencyValidationException actualHomeRequestDependencyValidationException =
                await Assert.ThrowsAsync<HomeRequestDependencyValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestDependencyValidationException.Should().BeEquivalentTo(
                expectedHomeRequestDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            HomeRequest someHomeRequest = CreateRandomHomeRequest();
            var dbUpdateConcurrencyException = new DBUConcurrencyException();
            var lockedHomeRequestException = new LockedHomeRequestException(dbConcurrencyException);

            var expectedHomeRequestDependencyValidationException =
                new HomeRequestDependencyValidationException(lockedHomeRequestException);

            this.storageBrokerMock.Setup(broker => broker.InsertHomeRequestAsync(
                It.IsAny<HomeRequest>())).ThrowsAsync(dbConcurrencyException);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(someHomeRequest);

            HomeRequestDependencyValidationException actualHomeRequestDependencyValidationException =
                await Assert.ThrowsAsync<HomeRequestDependencyValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestDependencyValidationException.Should().BeEquivalentTo(
                expectedHomeRequestDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}