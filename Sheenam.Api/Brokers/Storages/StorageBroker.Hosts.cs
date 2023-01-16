//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Microsoft.EntityFrameworkCore;
using Sheenam.Api.Models.Foundations.Hosts;
using System.Linq;
using System.Threading.Tasks;

namespace Sheenam.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        DbSet<Host> Hosts { get; set; }

        public async ValueTask<Host> InsertHostAsync(Host host) =>
            await InsertAsync(host);

        public IQueryable<Host> SelectAllHosts() =>
            SelectAll<Host>();
    }
}