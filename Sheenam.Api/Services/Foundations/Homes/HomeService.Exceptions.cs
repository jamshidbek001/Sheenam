//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
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
            catch(InvalidHomeException invalidHomeException)
            {
                throw CreateAndLogValidationException(invalidHomeException);
            }
        }

        private HomeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var homeValidationException =
                    new HomeValidationException(exception);

            this.loggingBroker.LogError(homeValidationException);

            return homeValidationException;
        }
    }
}