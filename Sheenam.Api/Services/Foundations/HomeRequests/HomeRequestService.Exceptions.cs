﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Xeptions;

namespace Sheenam.Api.Services.Foundations.HomeRequests
{
    public partial class HomeRequestService
    {
        private delegate ValueTask<HomeRequest> ReturningHomeRequestFunction();

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
                AlreadyExistHomeRequestException alreadyExistHomeRequestException =
                    new AlreadyExistHomeRequestException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistHomeRequestException);
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
    }
}