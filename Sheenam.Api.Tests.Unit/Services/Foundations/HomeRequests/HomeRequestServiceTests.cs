//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using Moq;
using Sheenam.Api.Brokers.DateTimes;
using Sheenam.Api.Brokers.Loggings;
using Sheenam.Api.Brokers.Storages;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Services.Foundations.HomeRequests;
using Tynamix.ObjectFiller;

namespace Sheenam.Api.Tests.Unit.Services.Foundations.HomeRequests
{
    public partial class HomeRequestServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHomeRequestService homeRequestService;

        public HomeRequestServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.homeRequestService = new HomeRequestService(
                this.storageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static HomeRequest CreateRandomHomeRequest() =>
            CreateHomeRequestFiller(GetRandomDateTime()).Create();
        private static HomeRequest CreateRandomHomeRequest(DateTimeOffset dates) =>
            CreateHomeRequestFiller(dates).Create();

        private static Filler<HomeRequest> CreateHomeRequestFiller(DateTimeOffset dates)
        {
            var filler = new Filler<HomeRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}