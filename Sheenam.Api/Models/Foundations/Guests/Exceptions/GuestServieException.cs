//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Guests.Exceptions
{
    public class GuestServieException : Xeption
    {
        public GuestServieException(Exception innerException)
            : base(message: "Guest service error occurred, contact support",
                 innerException)
        { }
    }
}
