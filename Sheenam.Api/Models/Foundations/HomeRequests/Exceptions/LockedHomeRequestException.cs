//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.HomeRequests.Exceptions
{
    public class LockedHomeRequestException : Xeption
    {
        public LockedHomeRequestException(Exception innerException)
            : base(message: "Home request is locked,please try again", innerException)
        { }
    }
}