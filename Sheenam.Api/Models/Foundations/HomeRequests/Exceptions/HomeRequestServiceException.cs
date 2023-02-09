//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.HomeRequests.Exceptions
{
    public class HomeRequestServiceException : Xeption
    {
        public HomeRequestServiceException(Exception innerException)
            : base(message: "Home request sercice error occurred,contact support",
                 innerException)
        { }
    }
}