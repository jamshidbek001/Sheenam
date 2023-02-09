//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.HomeRequests.Exceptions
{
    public class HomeRequestDependencyValidationException : Xeption
    {
        public HomeRequestDependencyValidationException(Xeption innerException)
            : base(message: "Home request validation error occurred,fix the errors and try again",
                 innerException)
        { }
    }
}