//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.HomeRequests
{
    public partial class HomeRequestService
    {
        private delegate ValueTask<HomeRequest> ReturningHomeRequestFunction();
        private delegate IQueryable<HomeRequest> ReturningHomeRequestFunctions();

        private async ValueTask<HomeRequest> TryCatch(ReturningHomeRequestFunction returningHomeRequestFunction)
        {
            try
            {
                return await returningHomeRequestFunction();
            }
            catch (NullHomeRequestException nullHomeRequestException)
            {
                throw CreateAndLogValidationException(nullHomeRequestException);
            }
            catch (InvalidHomeRequestException invalidHomeRequestException)
            {
                throw CreateAndLogValidationException(invalidHomeRequestException);
            }
            catch (SqlException sqlException)
            {
                var failedHomeRequestStorageException = new FailedHomeRequestStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHomeRequestStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistHomeRequestException =
                    new AlreadyExistHomeRequestException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistHomeRequestException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedHomeRequestException =
                    new LockedHomeRequestException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedHomeRequestException);
            }
            catch (Exception serviceException)
            {
                var failedHomeRequestServiceException = new FailedHomeRequestServiceException(serviceException);

                throw CreateAndLogServiceException(failedHomeRequestServiceException);
            }
        }

        private IQueryable<HomeRequest> TryCatch(ReturningHomeRequestFunctions returningHomeRequestFunctions)
        {
            try
            {
                return returningHomeRequestFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedHomeRequestServiceException =
                    new FailedHomeRequestServiceException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHomeRequestServiceException);
            }
            catch (Exception serviceException)
            {
                var failedHomeRequestServiceException =
                    new FailedHomeRequestServiceException(serviceException);

                throw CreateAndLogServiceException(failedHomeRequestServiceException);
            }
        }

        private HomeRequestValidationException CreateAndLogValidationException(Xeption exception)
        {
            var homeRequestValidationException = new HomeRequestValidationException(exception);
            this.loggingBroker.LogError(homeRequestValidationException);

            return homeRequestValidationException;
        }

        private HomeRequestDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var homeRequestDependencyException = new HomeRequestDependencyException(exception);
            this.loggingBroker.LogCritical(homeRequestDependencyException);

            return homeRequestDependencyException;
        }

        private HomeRequestDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var homeRequestDependencyValidationException =
                new HomeRequestDependencyValidationException(exception);

            this.loggingBroker.LogError(homeRequestDependencyValidationException);

            return homeRequestDependencyValidationException;
        }

        private Exception CreateAndLogServiceException(Xeption exception)
        {
            var homeRequestServiceException =
                new HomeRequestServiceException(exception);

            this.loggingBroker.LogError(homeRequestServiceException);

            return homeRequestServiceException;
        }
    }
}