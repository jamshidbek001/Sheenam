//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;

namespace Sheenam.Api.Services.Foundations.HomeRequests
{
    public partial class HomeRequestService
    {
        private static void ValidateHomeRequest(HomeRequest homeRequest)
        {
            ValidateHomeRequestNotNull(homeRequest);

            Validate(
                (Rule: IsInvalid(homeRequest.Id), Parameter: nameof(HomeRequest.Id)),
                (Rule: IsInvalid(homeRequest.GuestId), Parameter: nameof(HomeRequest.GuestId)),
                (Rule: IsInvalid(homeRequest.HomeId), Parameter: nameof(HomeRequest.HomeId)),
                (Rule: IsInvalid(homeRequest.CreatedDate), Parameter: nameof(HomeRequest.CreatedDate)),
                (Rule: IsInvalid(homeRequest.UpdatedDate), Parameter: nameof(HomeRequest.UpdatedDate)));
        }

        private static void ValidateHomeRequestNotNull(HomeRequest homeRequest)
        {
            if (homeRequest is null)
            {
                throw new NullHomeRequestException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidHomeRequestException = new InvalidHomeRequestException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidHomeRequestException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidHomeRequestException.ThrowIfContainsErrors();
        }
    }
}