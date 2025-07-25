﻿using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Entities;
using System.Net.Sockets;
using Edoha.Domain.Models.DTOs.UserInstitution;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInstitutionController : ControllerBase
    {
        private readonly IUserInstitutionService _userInstitutionService;

        public UserInstitutionController(IUserInstitutionService userInstitutionService)
        {
            _userInstitutionService = userInstitutionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userInstitutions = await _userInstitutionService.SelectAllUserInstitutions();

                if (!userInstitutions.Any())
                {
                    return NoContent();
                }

                return Ok(userInstitutions);
            }catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userInstitution = await _userInstitutionService.SelectUserInstitutionById(id);

                if (userInstitution == null)
                {
                    return NoContent();
                }

                return Ok(userInstitution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserInstitutionDTO request)
        {
            if(request != null)
            {
                try
                {
                    await _userInstitutionService.InsertUserInstitution(request);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        InnerException = ex.InnerException?.Message
                    });
                }
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserInstitutionDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _userInstitutionService.UpdateUserInstitutionById(request);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        InnerException = ex.InnerException?.Message
                    });
                }
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid id)
        {
            await _userInstitutionService.DeleteUserInstitutionById(id);
        }
    }
}
