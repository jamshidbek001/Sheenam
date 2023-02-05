//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Homes
{
    public partial class HomeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveHomeIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid ivalidHomeId = Guid.Empty;
            var invalidHomeException = new InvalidHomeException();

            invalidHomeException.AddData(
                key: nameof(Home.Id),
                values: "Id is required");

            var expectedHomeValidationException =
                new HomeValidationException(invalidHomeException);

            // when
            ValueTask<Home> removeHomeByIdTask =
                this.homeService.RemoveHomeByIdAsync(ivalidHomeId);

            HomeValidationException actualHomeValidationException =
                await Assert.ThrowsAsync<HomeValidationException>(removeHomeByIdTask.AsTask);

            // then
            actualHomeValidationException.Should().BeEquivalentTo(
                expectedHomeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHomeAsync(It.IsAny<Home>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfHomeIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomHomeId = Guid.NewGuid();
            Guid inputHomeId = randomHomeId;
            Home noHome = null;
            var notFoundException = new NotFoundHomeException(inputHomeId);

            var expectedHomeValidationException =
                new HomeValidationException(notFoundException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noHome);

            // when
            ValueTask<Home> removeHomeTask =
                this.homeService.RemoveHomeByIdAsync(inputHomeId);

            HomeValidationException actualHomeValidationException =
                await Assert.ThrowsAsync<HomeValidationException>(removeHomeTask.AsTask);

            // then
            actualHomeValidationException.Should().BeEquivalentTo(
                expectedHomeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHomeAsync(It.IsAny<Home>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}