//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;

namespace Sheenam.Api.Services.Foundations.Homes
{
    public partial class HomeService
    {
        private void ValidateHomeNotNull(Home home)
        {
            if (home is null)
            {
                throw new NullHomeException();
            }
        }
    }
}