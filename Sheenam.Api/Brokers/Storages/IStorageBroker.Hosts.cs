//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Sheenam.Api.Models.Foundations.Hosts;
using System.Linq;
using System.Threading.Tasks;

namespace Sheenam.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Host> InsertHostAsync(Host host);
        IQueryable<Host> SelectAllHosts();
    }
}