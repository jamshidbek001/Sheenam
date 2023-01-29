using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Hosts
{
    public partial class HostServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlErrorOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlError();
            var failedHostServiceException = new FailedHostServiceException(sqlException);

            var expectedHostDependencyException =
                new HostDependencyException(failedHostServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHosts()).Throws(sqlException);

            // when
            Action retrieveAllHostAction = () => this.hostService.RetrieveAllHosts();

            HostDependencyException actualHostDependencyException =
                Assert.Throws<HostDependencyException>(retrieveAllHostAction);

            // then
            actualHostDependencyException.Should().BeEquivalentTo(expectedHostDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHosts(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedHostDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccurredAndLogIt()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);
            var failedHostServiceException = new FailedHostServiceException(serviceException);
            var expectedHostServiceException = new HostServiceException(failedHostServiceException);

            this.storageBrokerMock.Setup(broker =>
            broker.SelectAllHosts()).Throws(serviceException);

            // when
            Action retrieveAllHostAction = () => this.hostService.RetrieveAllHosts();

            HostServiceException actualHostServiceException =
                Assert.Throws<HostServiceException>(retrieveAllHostAction);

            // then
            actualHostServiceException.Should().BeEquivalentTo(expectedHostServiceException);

            this.storageBrokerMock.Verify(broker => broker.SelectAllHosts(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedHostServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}