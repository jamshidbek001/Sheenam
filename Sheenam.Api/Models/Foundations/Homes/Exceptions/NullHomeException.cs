//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Xeptions;

namespace Sheenam.Api.Models.Foundations.Homes.Exceptions
{
    public class NullHomeException : Xeption
    {
        public NullHomeException()
            : base(message: "Home is null")
        { }
    }
}