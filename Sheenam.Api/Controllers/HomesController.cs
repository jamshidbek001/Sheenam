//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Sheenam.Api.Models.Foundations.Homes;
using Sheenam.Api.Models.Foundations.Homes.Exceptions;
using Sheenam.Api.Services.Foundations.Homes;

namespace Sheenam.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomesController : RESTFulController
    {
        private readonly IHomeService homeService;

        public HomesController(IHomeService homeService) =>
            this.homeService = homeService;

        [HttpPost]
        public async ValueTask<ActionResult<Home>> PostHomeAsync(Home home)
        {
            try
            {
                return await this.homeService.AddHomeAsync(home);
            }
            catch (HomeValidationException homeValidationException)
            {
                return BadRequest(homeValidationException.InnerException);
            }
            catch (HomeDependencyValidationException homeDependencyValidationException)
                when (homeDependencyValidationException.InnerException is AlreadyExistsHomeException)
            {
                return Conflict(homeDependencyValidationException.InnerException);
            }
            catch (HomeDependencyValidationException homeDependencyValidationException)
            {
                return BadRequest(homeDependencyValidationException.InnerException);
            }
            catch (HomeDependencyException homeDependencyException)
            {
                return InternalServerError(homeDependencyException.InnerException);
            }
            catch (HomeServiceException homeServiceException)
            {
                return InternalServerError(homeServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Home>> GetAllHomes()
        {
            try
            {
                IQueryable<Home> allHomes = this.homeService.RetrieveAllHomes();

                return Ok(allHomes);
            }
            catch (HomeDependencyException homeDependencyException)
            {
                return InternalServerError(homeDependencyException.InnerException);
            }
            catch (HomeServiceException homeServiceException)
            {
                return InternalServerError(homeServiceException.InnerException);
            }
        }

        [HttpGet("{homeId}")]
        public async ValueTask<ActionResult<Home>> GetHomeByIdAsync(Guid id)
        {
            try
            {
                return await this.homeService.RetrieveHomeByIdAsync(id);
            }
            catch (HomeDependencyException homeDependencyException)
            {
                return InternalServerError(homeDependencyException.InnerException);
            }
            catch (HomeValidationException homeValidationException)
                when (homeValidationException.InnerException is InvalidHomeException)
            {
                return BadRequest(homeValidationException.InnerException);
            }
            catch (HomeValidationException homeValidationException)
                when (homeValidationException.InnerException is NotFoundHomeException)
            {
                return NotFound(homeValidationException.InnerException);
            }
            catch (HomeServiceException homeServiceException)
            {
                return InternalServerError(homeServiceException.InnerException);
            }
        }
    }
}