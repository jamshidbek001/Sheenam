//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class LockedHomeException : Xeption
    {
        public LockedHomeException(Exception innerException)
            : base(message: "Home is locked,please try again.", innerException)
        { }
    }
}