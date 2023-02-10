//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Linq;
using System.Threading.Tasks;
using Sheenam.Api.Models.Foundations.HomeRequests;

namespace Sheenam.Api.Services.Foundations.HomeRequests
{
    public interface IHomeRequestService
    {
        ValueTask<HomeRequest> AddHomeRequstAsync(HomeRequest homeRequest);
        IQueryable<HomeRequest> RetrieveAllHomeRequests();
    }
}