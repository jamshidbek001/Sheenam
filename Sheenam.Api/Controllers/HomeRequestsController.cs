//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Sheenam.Api.Models.Foundations.HomeRequests;
using Sheenam.Api.Models.Foundations.HomeRequests.Exceptions;
using Sheenam.Api.Services.Foundations.HomeRequests;

namespace Sheenam.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeRequestsController : RESTFulController
    {
        private readonly IHomeRequestService homeRequestService;

        public HomeRequestsController(IHomeRequestService homeRequestService) =>
            this.homeRequestService = homeRequestService;

        [HttpPost]
        public async ValueTask<ActionResult<HomeRequest>> PostHomeRequestAsync(HomeRequest homeRequest)
        {
            try
            {
                HomeRequest postedHomeRequest = await this.homeRequestService.AddHomeRequstAsync(homeRequest);

                return Created(postedHomeRequest);
            }
            catch (HomeRequestValidationException homeRequestValidationException)
            {
                return BadRequest(homeRequestValidationException.InnerException);
            }
            catch (HomeRequestDependencyValidationException homeRequestDependencyValidationException)
                when (homeRequestDependencyValidationException.InnerException is AlreadyExistHomeRequestException)
            {
                return Conflict(homeRequestDependencyValidationException.InnerException);
            }
            catch (HomeRequestDependencyValidationException homeRequestDependencyValidationException)
            {
                return BadRequest(homeRequestDependencyValidationException.InnerException);
            }
            catch (HomeRequestDependencyException homeRequestDependencyException)
            {
                return InternalServerError(homeRequestDependencyException.InnerException);
            }
            catch (HomeRequestServiceException homeRequestServiceException)
            {
                return InternalServerError(homeRequestServiceException.InnerException);
            }
        }
    }
}