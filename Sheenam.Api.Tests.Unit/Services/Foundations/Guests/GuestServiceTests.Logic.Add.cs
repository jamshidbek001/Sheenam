//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using FluentAssertions;
using Moq;
using Sheenam.Api.Models.Foundations.Guests;
using Xunit;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.Guests
{
    public partial class GuestServiceTests
    {
        [Fact]
        public async Task ShouldAddGuestInWrongWayAsync()
        {
            // arrange
            Guest randomGuest = new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = "James",
                LastName = "Alex",
                Address = "Lasek Brzozowy 1",
                DateOfBirth = new DateTimeOffset(),
                Email = "random@gmail.com",
                Gender = GenderType.Male,
                PhoneNumber = "517520208"
            };

            this.storageBrokerMock.Setup(broker =>
                broker.InsertGuestAsync(randomGuest))
                    .ReturnsAsync(randomGuest);
            // act
            Guest actual = await this.guestService.AddGuestAsync(randomGuest);
            //assert
            actual.Should().BeEquivalentTo(randomGuest);
        }

        [Fact]
        public async Task ShoulAddGuestAsync()
        {
            // given
            Guest randomGuest = CreateRandomGuest();
            Guest inputGuest = randomGuest;
            Guest returningGuest = inputGuest;
            Guest expectedGuest = returningGuest;

            this.storageBrokerMock.Setup(broker=>
            broker.InsertGuestAsync(inputGuest))
                .ReturnsAsync(returningGuest);

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