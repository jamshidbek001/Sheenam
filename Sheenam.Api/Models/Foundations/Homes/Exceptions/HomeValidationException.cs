//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class HomeValidationException : Xeption
    {
        public HomeValidationException(Xeption innerException)
            : base(message: "Home validation error occurred, fix the errors and try again",
                  innerException)
        { }
    }
}