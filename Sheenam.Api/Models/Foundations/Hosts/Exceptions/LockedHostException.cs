//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Hosts.Exceptions
{
    public class LockedHostException : Xeption
    {
        public LockedHostException(Exception innerException)
            : base(message: "Host is locked,please try again", innerException)
        { }
    }
}