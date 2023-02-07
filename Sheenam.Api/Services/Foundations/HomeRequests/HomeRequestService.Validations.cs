//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;

namespace Sheenam.Api.Services.Foundations.HomeRequests
{
    public partial class HomeRequestService
    {
        private static void ValidateHomeRequestNotNull(HomeRequest homeRequest)
        {
            if (homeRequest is null)
            {
                throw new NullHomeRequestException();
            }
        }
    }
}