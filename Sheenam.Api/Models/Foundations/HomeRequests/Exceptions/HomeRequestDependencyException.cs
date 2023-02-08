//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.HomeRequests.Exceptions
{
    public partial class HomeRequestDependencyException : Xeption
    {
        public HomeRequestDependencyException(Xeption innerException)
            : base(message: "Home request dependency error occurred,contact support",
                 innerException)
        { }
    }
}