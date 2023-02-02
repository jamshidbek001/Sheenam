//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Sheenam.Api.Models.Foundations.Homes;

namespace Sheenam.Api.Services.Foundations.Homes
{
    public interface IHomeService
    {
        ValueTask<Home> AddHomeAsync(Home home);
        IQueryable<Home> RetrieveAllHomes();
        ValueTask<Home> RetrieveHomeByIdAsync(Guid homeId);
    }
}