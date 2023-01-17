//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Hosts.Exceptions
{
    public class FailedHostStorageException : Xeption
    {
        public FailedHostStorageException(Exception innerException)
            : base(message: "Failed host storage error occurred, contact support", innerException)
        { }
    }
}