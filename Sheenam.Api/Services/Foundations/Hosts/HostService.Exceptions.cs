//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Sheenam.Api.Models.Foundations.Hosts;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.Hosts
{
    public partial class HostService
    {
        private delegate ValueTask<Host> ReturningHostFunction();
        private delegate IQueryable<Host> ReturningHostFunctions();

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
            catch (NotFoundHostException notFoundHostException)
            {
                throw CreateAndLogValidationException(notFoundHostException);
            }
            catch (SqlException sqlException)
            {
                var failedHostStorageException = new FailedHostStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHostStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistHostException = new AlreadyExistHostException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistHostException);
            }
            catch (Exception serviceException)
            {
                var failedHostServiceException = new FailedHostServiceException(serviceException);

                throw CreateAndLogServiceException(failedHostServiceException);
            }
        }

        private IQueryable<Host> TryCatch(ReturningHostFunctions returningHostFunctions)
        {
            try
            {
                return returningHostFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedServiceException = new FailedHostServiceException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedServiceException);
            }
            catch (Exception serviceException)
            {
                var failedServiceHostException = new FailedHostServiceException(serviceException);

                throw CreateAndLogServiceException(failedServiceHostException);
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

        private HostDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var hostDependencyValidationException = new HostDependencyValidationException(exception);
            this.loggingBroker.LogError(hostDependencyValidationException);

            return hostDependencyValidationException;
        }

        private HostServiceException CreateAndLogServiceException(Xeption exception)
        {
            var hostServiceException = new HostServiceException(exception);
            this.loggingBroker.LogError(hostServiceException);

            return hostServiceException;
        }
    }
}