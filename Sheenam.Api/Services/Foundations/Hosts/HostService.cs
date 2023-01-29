//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Sheenam.Api.Brokers.DateTimes;
using Sheenam.Api.Brokers.Loggings;
using Sheenam.Api.Brokers.Storages;
using Sheenam.Api.Models.Foundations.Hosts;

namespace Sheenam.Api.Services.Foundations.Hosts
{
    public partial class HostService : IHostService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public HostService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Host> AddHostAsync(Host host) =>
        TryCatch(async () =>
        {
            ValidateHostOnAdd(host);

            return await this.storageBroker.InsertHostAsync(host);
        });

        public IQueryable<Host> RetrieveAllHosts() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllHosts();
        });

        public ValueTask<Host> RetrieveHostByIdAsync(Guid hostId) =>
        TryCatch(async () =>
        {
            ValidateHostId(hostId);

            Host maybeHost =
                await this.storageBroker.SelectHostByIdAsync(hostId);

            ValidateStorageHost(maybeHost, hostId);

            return maybeHost;
        });
    }
}