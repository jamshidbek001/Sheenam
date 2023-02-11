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
        public async Task ShouldThrowValidationExceptionOnAddIfHomeRequestIsNullAndLogItAsync()
        {
            // given
            HomeRequest noHomeRequest = null;
            var nullHomeRequestException = new NullHomeRequestException();

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(nullHomeRequestException);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(noHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfHomeRequestIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidHomeRequest = new HomeRequest
            {
                Message = invalidString
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
                key: nameof(HomeRequest.Message),
                values: "Text is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.CreatedDate),
                values: "Value is required");

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.UpdatedDate),
                values: "Value is required");

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(invalidHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            DateTimeOffset anotherRandomDate = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest(randomDateTime);
            HomeRequest invalidHomeRequest = randomHomeRequest;
            invalidHomeRequest.UpdatedDate = anotherRandomDate;
            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.CreatedDate),
                values: $"Date is not same as {nameof(HomeRequest.UpdatedDate)}");

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(invalidHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            DateTimeOffset invalidRandomDateTime = randomDateTime.AddSeconds(invalidSeconds);
            HomeRequest randomInvalidHomeRequest = CreateRandomHomeRequest(invalidRandomDateTime);
            HomeRequest inalidHomeRequest = randomInvalidHomeRequest;
            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.CreatedDate),
                values: "Date is not recent");

            var expectedHomeRequestValidationException =
                new HomeRequestValidationException(invalidHomeRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<HomeRequest> addHomeRequestTask =
                this.homeRequestService.AddHomeRequstAsync(inalidHomeRequest);

            HomeRequestValidationException actualHomeRequestValidationException =
                await Assert.ThrowsAsync<HomeRequestValidationException>(addHomeRequestTask.AsTask);

            // then
            actualHomeRequestValidationException.Should().BeEquivalentTo(
                expectedHomeRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(It.IsAny<HomeRequest>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}