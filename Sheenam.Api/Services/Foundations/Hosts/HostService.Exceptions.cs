//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Sheenam.Api.Models.Foundations.Hosts;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.Hosts
{
    public partial class HostService
    {
        private delegate ValueTask<Host> ReturningHostFunction();

        private async ValueTask<Host> TryCatch(ReturningHostFunction returningHostFunction)
        {
            try
            {
                return await returningHostFunction();
            }
            catch (NullHostException nullHostException)
            {
                throw CreateAndLogValidationException(nullHostException);
            }
            catch (InvalidHostException invalidHostException)
            {
                throw CreateAndLogValidationException(invalidHostException);
            }
            catch (SqlException sqlException)
            {
                var failedHostStorageException = new FailedHostStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHostStorageException);
            }
        }

        private HostValidationException CreateAndLogValidationException(Xeption exception)
        {
            var hostValidationException =
                    new HostValidationException(exception);

            this.loggingBroker.LogError(hostValidationException);

            return hostValidationException;
        }

        private HostDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var hostDependencyException = new HostDependencyException(exception);
            this.loggingBroker.LogCritical(hostDependencyException);

            return hostDependencyException;
        }
    }
}