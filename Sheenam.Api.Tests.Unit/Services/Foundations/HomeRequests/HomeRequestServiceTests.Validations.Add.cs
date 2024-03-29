﻿//=================================
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
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfHomeRequestIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidGuid = Guid.Empty;

            var invalidHomeRequest = new HomeRequest
            {
                Id = invalidGuid
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
                values: "Date is required");

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedHomeRequestValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHomeRequestAsync(invalidHomeRequest), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            HomeRequest randomHomeRequest = CreateRandomHomeRequest(randomDateTime);
            HomeRequest invalidHomeRequest = randomHomeRequest;
            invalidHomeRequest.UpdatedDate = invalidHomeRequest.CreatedDate.AddDays(randomNumber);
            var invalidHomeRequestException = new InvalidHomeRequestException();

            invalidHomeRequestException.AddData(
                key: nameof(HomeRequest.UpdatedDate),
                values: $"Date is not the same as {nameof(HomeRequest.CreatedDate)}");

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