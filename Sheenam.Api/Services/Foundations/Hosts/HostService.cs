//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using Sheenam.Api.Brokers.Loggings;
using Sheenam.Api.Brokers.Storages;
using Sheenam.Api.Models.Foundations.Hosts;
using Sheenam.Api.Models.Foundations.Hosts.Exceptions;

namespace Sheenam.Api.Services.Foundations.Hosts
{
    public class HostService : IHostService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public HostService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Host> AddHostAsync(Host host)
        {
            try
            {
                if (host is null)
                {
                    throw new NullHostException();
                }

                return await this.storageBroker.InsertHostAsync(host);

            }
            catch (NullHostException nullHostException)
            {
                var hostValidationException =
                    new HostValidationException(nullHostException);

                this.loggingBroker.LogError(hostValidationException);

                throw hostValidationException;
            }
        }
    }
}