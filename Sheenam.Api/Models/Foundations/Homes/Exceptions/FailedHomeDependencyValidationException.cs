//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class FailedHomeDependencyValidationException : Xeption
    {
        public FailedHomeDependencyValidationException(Exception innerException)
            : base(message: "Failed home dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}