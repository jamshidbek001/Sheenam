//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someHomeRequestId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedHomeRequestStorageException =
                new FailedHomeRequestStorageException(sqlException);

            var excpectedHomeRequestDependencyException =
                new HomeRequestDependencyException(failedHomeRequestStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeRequestByIdAsync(It.IsAny<Guid>())).ThrowsAsync(sqlException);

            // when
            ValueTask<HomeRequest> removeHomeRequestTask =
                this.homeRequestService.RemoveHomeRequestByIdAsync(someHomeRequestId);

            HomeRequestDependencyException actualHomeRequestDependencyException =
                await Assert.ThrowsAsync<HomeRequestDependencyException>(removeHomeRequestTask.AsTask);

            // then
            actualHomeRequestDependencyException.Should().BeEquivalentTo(
                excpectedHomeRequestDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    excpectedHomeRequestDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveIfDatabaseUpdateConcurrencyExceptionAndLogItAsync()
        {
            // given
            Guid someHomeRequestId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedHomeRequestException = new LockedHomeRequestException(dbUpdateConcurrencyException);

            var excpectedHomeRequestDependencyValidationException =
                new HomeRequestDependencyValidationException(lockedHomeRequestException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeRequestByIdAsync(It.IsAny<Guid>())).ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<HomeRequest> removeHomeRequestTask =
                this.homeRequestService.RemoveHomeRequestByIdAsync(someHomeRequestId);

            HomeRequestDependencyValidationException actualHomeRequestDependencyValidationException =
                await Assert.ThrowsAsync<HomeRequestDependencyValidationException>(removeHomeRequestTask.AsTask);

            // then
            actualHomeRequestDependencyValidationException.Should().BeEquivalentTo(
                excpectedHomeRequestDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedHomeRequestDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}