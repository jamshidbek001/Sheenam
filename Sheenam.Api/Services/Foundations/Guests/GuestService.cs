//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Sheenam.Api.Brokers.Loggings;
using Sheenam.Api.Brokers.Storages;
using Sheenam.Api.Models.Foundations.Guests;

namespace Sheenam.Api.Services.Foundations.Guests
{
    public partial class GuestService : IGuestService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public GuestService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Guest> AddGuestAsync(Guest guest) =>
        TryCatch(async () =>
        {
            ValidateGuestOnAdd(guest);

            return await this.storageBroker.InsertGuestAsync(guest);
        });

        public IQueryable<Guest> RetrieveAllGuests() =>
        TryCatch(() => this.storageBroker.SelectAllGuests());

        public ValueTask<Guest> RetrieveGuestByIdAsync(Guid guestId) =>
        TryCatch(async () =>
        {
            ValidateGuestId(guestId);

            Guest maybeGuest =
                await this.storageBroker.SelectGuestByIdAsync(guestId);
            
            ValidateStorageGuest(maybeGuest,guestId);

            return maybeGuest;
        });

        public ValueTask<Guest> ModifyGuestAsync(Guest guest) =>
        TryCatch(async () =>
        {
            ValidateGuestOnModify(guest);
            var maybeGuest = await this.storageBroker.SelectGuestByIdAsync(guest.Id);

            ValidateStorageGuest(maybeGuest,guest.Id);

            return await this.storageBroker.UpdateGuestAsync(guest);
        });

        public ValueTask<Guest> RemoveGuestByIdAsync(Guid guestId) =>
            throw new NotImplementedException();
    }
}
