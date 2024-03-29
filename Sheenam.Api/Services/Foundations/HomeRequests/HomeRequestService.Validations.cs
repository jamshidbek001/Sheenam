﻿//=================================
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
        private void ValidateHomeRequest(HomeRequest homeRequest)
        {
            ValidateHomeRequestNotNull(homeRequest);

            Validate(
                (Rule: IsInvalid(homeRequest.Id), Parameter: nameof(HomeRequest.Id)),
                (Rule: IsInvalid(homeRequest.GuestId), Parameter: nameof(HomeRequest.GuestId)),
                (Rule: IsInvalid(homeRequest.HomeId), Parameter: nameof(HomeRequest.HomeId)),
                (Rule: IsInvalid(homeRequest.CreatedDate), Parameter: nameof(HomeRequest.CreatedDate)),
                (Rule: IsInvalid(homeRequest.UpdatedDate), Parameter: nameof(HomeRequest.UpdatedDate)),
                (Rule: IsNotRecent(homeRequest.CreatedDate), Parameter: nameof(HomeRequest.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: homeRequest.UpdatedDate,
                    secondDate: homeRequest.CreatedDate,
                    secondDateName: nameof(HomeRequest.CreatedDate)),
                    Parameter: nameof(HomeRequest.UpdatedDate)));
        }

        private void ValidateHomeRequestOnModify(HomeRequest homeRequest)
        {
            ValidateHomeRequestNotNull(homeRequest);

            Validate(
                (Rule: IsInvalid(homeRequest.Id), Parameter: nameof(HomeRequest.Id)),
                (Rule: IsInvalid(homeRequest.GuestId), Parameter: nameof(HomeRequest.GuestId)),
                (Rule: IsInvalid(homeRequest.HomeId), Parameter: nameof(HomeRequest.HomeId)),
                (Rule: IsInvalid(homeRequest.CreatedDate), Parameter: nameof(HomeRequest.CreatedDate)),
                (Rule: IsInvalid(homeRequest.UpdatedDate), Parameter: nameof(HomeRequest.UpdatedDate)),
                (Rule: IsNotRecent(homeRequest.UpdatedDate), Parameter: nameof(HomeRequest.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: homeRequest.UpdatedDate,
                    secondDate: homeRequest.CreatedDate,
                    secondDateName: nameof(homeRequest.CreatedDate)),

                    Parameter: nameof(HomeRequest.UpdatedDate)));
        }

        private static void ValidateAgainstStorageHomeRequestOnModify(
            HomeRequest inputHomeRequest,
            HomeRequest storageHomeRequest)
        {
            ValidateStorageHomeRequest(storageHomeRequest, inputHomeRequest.Id);

            Validate(
                (Rule: IsNotSame(
                    firstDate: inputHomeRequest.CreatedDate,
                    secondDate: storageHomeRequest.CreatedDate,
                    secondDateName: nameof(inputHomeRequest.CreatedDate)),

                    Parameter: nameof(HomeRequest.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputHomeRequest.UpdatedDate,
                    secondDate: storageHomeRequest.UpdatedDate,
                    secondDateName: nameof(HomeRequest.UpdatedDate)),

                    Parameter: nameof(HomeRequest.UpdatedDate)));
        }

        private static void ValidateHomeRequestNotNull(HomeRequest homeRequest)
        {
            if (homeRequest is null)
            {
                throw new NullHomeRequestException();
            }
        }

        private static void ValidateHomeRequestId(Guid homeRequestId) =>
            Validate((Rule: IsInvalid(homeRequestId), Parameter: nameof(HomeRequest.Id)));

        private static void ValidateStorageHomeRequest(HomeRequest maybeHomeRequest, Guid homeRequestId)
        {
            if (maybeHomeRequest is null)
            {
                throw new NotFoundHomeRequestException(homeRequestId);
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
            Message = "Date is required"
        };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrentDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
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