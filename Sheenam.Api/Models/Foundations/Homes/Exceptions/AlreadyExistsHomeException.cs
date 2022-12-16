//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class AlreadyExistsHomeException : Xeption
    {
        public AlreadyExistsHomeException(Exception innerException)
            : base(message: "Home already exists.",
                  innerException)
        { }
    }
}