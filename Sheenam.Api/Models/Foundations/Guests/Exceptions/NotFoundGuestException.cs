//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Guests.Exceptions
{
    public class NotFoundGuestException : Xeption
    {
        public NotFoundGuestException(Guid guestId)
            : base(message: $"Could not found guest with id:{guestId}.")
        { }
    }
}