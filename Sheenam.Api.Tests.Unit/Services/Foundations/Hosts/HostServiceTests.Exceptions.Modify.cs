//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.Hosts;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Hosts
{
    public partial class HostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Host someHost = CreateRandomHost();
            SqlException sqlException = GetSqlError();
            var failedHostStorageException = new FailedHostStorageException(sqlException);

            var expectedHostDependencyException =
                new HostDependencyException(failedHostStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHostByIdAsync(someHost.Id)).ThrowsAsync(sqlException);

            // when
            ValueTask<Host> modifyHostTask =
                this.hostService.ModifyHostAsync(someHost);

            HostDependencyException actualHostDependencyException =
                await Assert.ThrowsAsync<HostDependencyException>(modifyHostTask.AsTask);

            // then
            actualHostDependencyException.Should().BeEquivalentTo(expectedHostDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHostByIdAsync(someHost.Id), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHostAsync(It.IsAny<Host>()), Times.Never);

            this.loggingBrokerMock.Verify(broker => broker.LogCritical(It.Is(SameExceptionAs(
                expectedHostDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIHostDoesNotExistAndLogItAsync()
        {
            // given
            Host randomHost = CreateRandomHost();
            Host nonExistHost = randomHost;
            Host nullHost = null;
            var notFoundHostException = new NotFoundHostException(nonExistHost.Id);

            var expectedHostValidationException =
                new HostValidationException(notFoundHostException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHostByIdAsync(nonExistHost.Id)).ReturnsAsync(nullHost);

            // when
            ValueTask<Host> modifyHostTask =
                this.hostService.ModifyHostAsync(nonExistHost);

            HostValidationException actualHostValidationException =
                await Assert.ThrowsAsync<HostValidationException>(modifyHostTask.AsTask);

            // then
            actualHostValidationException.Should().BeEquivalentTo(expectedHostValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHostByIdAsync(nonExistHost.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedHostValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}