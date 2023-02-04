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
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.Homes
{
    public partial class HomeService
    {
        private delegate ValueTask<Home> ReturningHomeFunction();
        private delegate IQueryable<Home> ReturningHomeFunctions();

        private async ValueTask<Home> TryCatch(ReturningHomeFunction returningHomeFunction)
        {
            try
            {
                return await returningHomeFunction();
            }
            catch (NullHomeException nullHomeException)
            {
                throw CreateAndLogValidationException(nullHomeException);
            }
            catch (InvalidHomeException invalidHomeException)
            {
                throw CreateAndLogValidationException(invalidHomeException);
            }
            catch (NotFoundHomeException notFoundHomeException)
            {
                throw CreateAndLogValidationException(notFoundHomeException);
            }
            catch (SqlException sqlException)
            {
                var failedHomeStorageException = new FailedHomeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHomeStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var failedHomeDependencyValidationException =
                    new AlreadyExistsHomeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(failedHomeDependencyValidationException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedHomeException = new LockedHomeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedHomeException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedHomeStorageException = new FailedHomeStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedHomeStorageException);
            }
            catch (Exception serviceException)
            {
                var failedHomeServiceException = new FailedHomeServiceException(serviceException);

                throw CreateAndLogServiceException(failedHomeServiceException);
            }
        }

        private IQueryable<Home> TryCatch(ReturningHomeFunctions returningHomeFunctions)
        {
            try
            {
                return returningHomeFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedStorageHomeException = new FailedHomeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStorageHomeException);
            }
            catch (Exception exception)
            {
                var faildHomeServiceException = new FailedHomeServiceException(exception);

                throw CreateAndLogServiceException(faildHomeServiceException);
            }
        }

        private HomeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var homeValidationException = new HomeValidationException(exception);
            this.loggingBroker.LogError(homeValidationException);

            return homeValidationException;
        }

        private HomeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var homeDependencyException = new HomeDependencyException(exception);
            this.loggingBroker.LogCritical(homeDependencyException);

            return homeDependencyException;
        }

        private HomeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var homeDependencyValidationException = new HomeDependencyValidationException(exception);
            this.loggingBroker.LogError(homeDependencyValidationException);

            return homeDependencyValidationException;
        }

        private HomeDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var homeDependencyException = new HomeDependencyException(exception);
            this.loggingBroker.LogError(homeDependencyException);

            return homeDependencyException;
        }

        private HomeServiceException CreateAndLogServiceException(Exception exception)
        {
            var homeServiceException = new HomeServiceException(exception);
            this.loggingBroker.LogError(homeServiceException);

            return homeServiceException;
        }
    }
}