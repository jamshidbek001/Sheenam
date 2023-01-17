//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Hosts.Exceptions
{
    public class AlreadyExistHostException : Xeption
    {
        public AlreadyExistHostException(Exception innerException)
            : base(message: "Host already exists", innerException)
        { }
    }
}