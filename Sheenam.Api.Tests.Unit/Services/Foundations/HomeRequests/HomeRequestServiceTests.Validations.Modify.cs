//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfHomeRequestIsNullAndLogItAsync()
        {
            // given
            HomeRequest nullHomeRequest = null;
            var nullHomeRequestException = new NullHomeRequestException();

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(nullHomeRequestException);

            // when
            ValueTask<HomeRequest> modifyHomeRequestTask =
                this.homeRequestService.ModifyHomeRequestAsync(nullHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(modifyHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHomeRequestAsync(nullHomeRequest), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        public async Task ShouldThrowValidationExceptionOnModifyIfHomeRequestIsInvalidAndLogItAsync(
            Guid invalidId)
        {
            // given
            var invalidHomeRequest = new HomeRequest
            {
                Id = invalidId
            };

            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.Id),
                values: "Id is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.GuestId),
                values: "Id is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.HomeId),
                values: "Id is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.CreatedDate),
                values: "Date is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.UpdatedDate),
                values: new[]
                {
                    "Date is required",
                    $"Date is the same as {nameof(HomeRequest.CreatedDate)}"
                });

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            // when
            ValueTask<HomeRequest> modifyHomeRequestTask =
                this.homeRequestService.ModifyHomeRequestAsync(invalidHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(modifyHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHomeRequestAsync(invalidHomeRequest), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest(randomDateTime);
            HomeRequest invalidHomeRequest = randomHomeRequest;
            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.UpdatedDate),
                values: $"Date is the same as {nameof(HomeRequest.CreatedDate)}");

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<HomeRequest> modifyHomeRequestTask =
                this.homeRequestService.ModifyHomeRequestAsync(invalidHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(modifyHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(invalidHomeRequest.Id), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest(dateTime);
            HomeRequest inputHomeRequest = randomHomeRequest;
            inputHomeRequest.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.UpdatedDate),
                values: "Date is not recent");

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            // when
            ValueTask<HomeRequest> modifyHomeRequestTask =
                this.homeRequestService.ModifyHomeRequestAsync(inputHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(modifyHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfHomeRequestDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest(dateTime);
            HomeRequest nonExistHomeRequest = randomHomeRequest;
            nonExistHomeRequest.CreatedDate = dateTime.AddMinutes(randomNegativeMinutes);
            HomeRequest nullHomeRequest = null;

            var notFoundHomeRequestException =
                new NotFoundHomeRequestException(nonExistHomeRequest.Id);

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(notFoundHomeRequestException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHomeRequestByIdAsync(nonExistHomeRequest.Id)).ReturnsAsync(nullHomeRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            // when
            ValueTask<HomeRequest> modifyHomeRequestTask =
                this.homeRequestService.ModifyHomeRequestAsync(nonExistHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(modifyHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHomeRequestByIdAsync(nonExistHomeRequest.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}