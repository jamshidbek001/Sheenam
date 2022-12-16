//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Home someHome = CreateRandomHome();
            SqlException sqlException = CreateSqlException();
            var failedHomeStorageException = new FailedHomeStorageException(sqlException);

            var expectedHomeDependencyExeption =
                new HomeDependencyException(failedHomeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHomeAsync(It.IsAny<Home>())).ThrowsAsync(sqlException);

            // when
            ValueTask<Home> addHomeTask = this.homeService.AddHomeAsync(someHome);

            HomeDependencyException actualHomeDependencyException =
                await Assert.ThrowsAsync<HomeDependencyException>(addHomeTask.AsTask);

            // then
            actualHomeDependencyException.Should().BeEquivalentTo(expectedHomeDependencyExeption);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeAsync(It.IsAny<Home>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHomeDependencyExeption))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            Home someHome = CreateRandomHome();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsHomeException =
                new AlreadyExistsHomeException(duplicateKeyException);

            var expectedHomeDependencyValidationException =
                new HomeDependencyValidationException(alreadyExistsHomeException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHomeAsync(It.IsAny<Home>())).ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Home> addHomeTask = this.homeService.AddHomeAsync(someHome);

            HomeDependencyValidationException actualHomeDependencyValidationException =
                await Assert.ThrowsAsync<HomeDependencyValidationException>(addHomeTask.AsTask);

            // then
            actualHomeDependencyValidationException.Should().BeEquivalentTo(expectedHomeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeAsync(It.IsAny<Home>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedHomeDependencyValidationException))),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}