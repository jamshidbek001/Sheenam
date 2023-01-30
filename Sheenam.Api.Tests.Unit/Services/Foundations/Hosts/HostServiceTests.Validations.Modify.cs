//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Hosts;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Hosts
{
    public partial class HostServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModiyIfHostIsNullAndLogItAsync()
        {
            // given
            Host nullHost = null;
            var nullHostException = new NullHostException();

            var expectedHostValidationException =
                new HostValidationException(nullHostException);

            // when
            ValueTask<Host> modifyHostTask =
                this.hostService.ModifyHostAsync(nullHost);

            HostValidationException actualHostValidationException =
                await Assert.ThrowsAsync<HostValidationException>(modifyHostTask.AsTask);

            // then
            actualHostValidationException.Should().BeEquivalentTo(expectedHostValidationException);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedHostValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHostByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}