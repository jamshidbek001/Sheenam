//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.HomeRequests.Exceptions
{
    public partial class FailedHomeRequestDependencyValidationException : Xeption
    {
        public FailedHomeRequestDependencyValidationException(Exception innerException)
            : base(message: "Home request dependency validation error occurred, fix the errors and try again",
                 innerException)
        { }
    }
}