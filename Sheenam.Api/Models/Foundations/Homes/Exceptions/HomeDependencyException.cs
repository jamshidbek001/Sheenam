//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class HomeDependencyException : Xeption
    {
        public HomeDependencyException(Xeption innerException)
            : base(message: "Home dependency error occurred, contact support",
                 innerException)
        { }
    }
}