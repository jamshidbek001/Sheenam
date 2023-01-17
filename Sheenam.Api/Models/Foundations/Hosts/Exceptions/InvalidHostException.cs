//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.Hosts.Exceptions
{
    public class InvalidHostException : Xeption
    {
        public InvalidHostException()
            : base(message: "Host is invalid")
        { }
    }
}