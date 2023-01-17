//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using Sheenam.Api.Models.Foundations.Hosts;

namespace Sheenam.Api.Services.Foundations.Hosts
{
    public interface IHostService
    {
        ValueTask<Host> AddHostAsync(Host host);
    }
}