//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public partial async Task ShoulAddGuestAsync()
        {
            // given
            Guest randomGuest = CreateRandomGuest();
            Guest inputGuest = randomGuest;
            Guest storageGuest = inputGuest;
            Guest expectedGuest = storageGuest.DeepClone();

            this.storageBrokerMock.Setup(broker=>
            broker.InsertGuestAsync(inputGuest))
                .ReturnsAsync(storageGuest);

            // when
            Guest actualGuest =
                await this.guestService.AddGuestAsync(inputGuest);
            // then
            actualGuest.Should().BeEquivalentTo(expectedGuest);

            this.storageBrokerMock.Verify(broker=>     // brokerni InsertGuestAsync metodi inputGuest yordamida
            broker.InsertGuestAsync(inputGuest),        // bir marta chaqirilsin
            Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();// storageBrokerMockni xar qanday bowqa Callarni oqlama
        }
    }
}