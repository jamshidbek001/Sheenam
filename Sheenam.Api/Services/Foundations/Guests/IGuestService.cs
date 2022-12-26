//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Sheenam.Api.Models.Foundations.Guests;

namespace Sheenam.Api.Services.Foundations.Guests
{
    public interface IGuestService
    {
        ValueTask<Guest> AddGuestAsync(Guest guest);
        IQueryable<Guest> RetrieveAllGuests();
        ValueTask<Guest> RetrieveGuestByIdAsync(Guid guestId);
        ValueTask<Guest> ModifyGuestAsync(Guest guest);
        ValueTask<Guest> RemoveGuestByIdAsync(Guid guestId);
    }
}
