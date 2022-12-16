//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.Homes
{
    public partial class HomeService
    {
        private delegate ValueTask<Home> ReturningHomeFunction();

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
            catch (SqlException sqlException)
            {
                var failedHomeStorageException = new FailedHomeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedHomeStorageException);
            }
            catch(DuplicateKeyException duplicateKeyException)
            {
                var failedHomeDependencyValidationException =
                    new FailedHomeDependencyValidationException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(failedHomeDependencyValidationException);
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
    }
}