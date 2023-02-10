//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllHomeRequests()
        {
            // given
            IQueryable<HomeRequest> randomHomeRequests = CreateRandomHomeRequests();
            IQueryable<HomeRequest> storageHomeRequests = randomHomeRequests;
            IQueryable<HomeRequest> excepectedHomeRequests = storageHomeRequests;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHomeRequests()).Returns(storageHomeRequests);

            // when
            IQueryable<HomeRequest> actualHomeRequest =
                this.homeRequestService.RetrieveAllHomeRequests();

            // then
            actualHomeRequest.Should().BeEquivalentTo(excepectedHomeRequests);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHomeRequests(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}