﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Homes
{
    public partial class HomeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();
            var failedHomeStorageException = new FailedHomeStorageException(sqlException);

            var expectedHomeDependencyException =
                new HomeDependencyException(failedHomeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>())).ThrowsAsync(sqlException);

            // when
            ValueTask<Home> retrieveHomeByIdTask =
                this.homeService.RetrieveHomeByIdAsync(someId);

            HomeDependencyException actualHomeDependencyException =
                await Assert.ThrowsAsync<HomeDependencyException>(retrieveHomeByIdTask.AsTask);

            // then
            actualHomeDependencyException.Should().BeEquivalentTo(expectedHomeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHomeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedHomeServiceException =
                new FailedHomeServiceException(serviceException);

            var expectedHomeServiceException =
                new HomeServiceException(failedHomeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<Home> retrieveHomeIdTask =
                this.homeService.RetrieveHomeByIdAsync(someId);

            HomeServiceException actualHomeServiceException =
                await Assert.ThrowsAsync<HomeServiceException>(retrieveHomeIdTask.AsTask);

            // then
            actualHomeServiceException.Should().BeEquivalentTo(expectedHomeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}