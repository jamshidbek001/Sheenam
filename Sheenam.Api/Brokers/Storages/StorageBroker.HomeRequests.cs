//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Microsoft.EntityFrameworkCore;
using Sheenam.Api.Models.Foundations.HomeRequests;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sheenam.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        DbSet<HomeRequest> HomeRequests { get; set; }

        public async ValueTask<HomeRequest> InsertHomeRequestAsync(HomeRequest homeRequest) =>
            await InsertAsync(homeRequest);

        public IQueryable<HomeRequest> SelectAllHomeRequests() =>
            SelectAll<HomeRequest>();

        public async ValueTask<HomeRequest> SelectHomeRequestByIdAsync(Guid id) =>
            await SelectAsync<HomeRequest>(id);
    }
}
