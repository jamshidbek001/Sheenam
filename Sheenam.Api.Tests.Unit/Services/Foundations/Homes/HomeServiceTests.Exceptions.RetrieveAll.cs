//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Homes
{
    public partial class HomeServiceTests
    {
        [Fact]
        public void ShouldThrowDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedHomeStorageException =
                new FailedHomeStorageException(sqlException);

            var expectedHomeDependencyException =
                new HomeDependencyException(failedHomeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHomes()).Throws(sqlException);

            // when
            Action retrieveAllHomeAction = () =>
                this.homeService.RetrieveAllHomes();

            HomeDependencyException actualHomeDependencyException =
                Assert.Throws<HomeDependencyException>(retrieveAllHomeAction);

            // then
            actualHomeDependencyException.Should().BeEquivalentTo(
                expectedHomeDependencyException);

            this.storageBrokerMock.Verify(broker => broker.SelectAllHomes());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHomeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}