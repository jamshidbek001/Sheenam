//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Homes
{
    public partial class HomeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Home someHome = CreateRandomHome();
            SqlException sqlException = CreateSqlException();

            var failedHomeStorageException =
                new FailedHomeStorageException(sqlException);

            var expectedHomeDependencyException =
               new HomeDependencyException(failedHomeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(someHome.Id)).ThrowsAsync(sqlException);

            // when
            ValueTask<Home> modifyHomeTask =
                this.homeService.ModifyHomeAsync(someHome);

            HomeDependencyException actualHomeDependencyException =
                await Assert.ThrowsAsync<HomeDependencyException>(modifyHomeTask.AsTask);

            // then
            actualHomeDependencyException.Should().BeEquivalentTo(
                expectedHomeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(someHome.Id), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHomeAsync(It.IsAny<Home>()), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHomeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfHomeDoesNotExistAndLogItAsync()
        {
            // given
            Home randomHome = CreateRandomHome();
            Home nonExistHome = randomHome;
            Home nullHome = null;

            var notFoundHomeException =
                new NotFoundHomeException(nonExistHome.Id);

            var expectedHomeValidationException =
                new HomeValidationException(notFoundHomeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(nonExistHome.Id)).ReturnsAsync(nullHome);
            // when
            ValueTask<Home> modifyHomeTask =
                this.homeService.ModifyHomeAsync(nonExistHome);

            HomeValidationException actualHomeValidationException =
                await Assert.ThrowsAsync<HomeValidationException>(modifyHomeTask.AsTask);

            // then
            actualHomeValidationException.Should().BeEquivalentTo(expectedHomeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(nonExistHome.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionAndLogItAsync()
        {
            // given
            Home randomHome = CreateRandomHome();
            Home someHome = randomHome;
            Guid homeId = someHome.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedHomeStorageException =
                new FailedHomeStorageException(databaseUpdateException);

            var expectedHomeDependencyException =
                new HomeDependencyException(failedHomeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(homeId)).ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<Home> modifyHomeTask =
                this.homeService.ModifyHomeAsync(someHome);

            HomeDependencyException actualHomeDependencyException =
                await Assert.ThrowsAsync<HomeDependencyException>(modifyHomeTask.AsTask);

            // then
            actualHomeDependencyException.Should().BeEquivalentTo(expectedHomeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(homeId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}