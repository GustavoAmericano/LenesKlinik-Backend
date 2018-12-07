using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.Entities;
using LenesKlinik.RestApi.DTO;
using LenesKlinik.RestApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LenesKlinik.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IUserService _userService;
        private IAuthenticationHelper authenticationHelper;
        
        public TokenController(
            IUserService userService,
            IAuthenticationHelper authService )
        {
            _userService = userService;
            authenticationHelper = authService;
        }


        [HttpPost]
        public ActionResult<string> Login([FromBody] LoginInput input)
        {
            var user = _userService.ValidateUser(input.Email, input.Password);

            // Authentication successful
            return Ok(new
            {
                username = user.Email,
                token = authenticationHelper.GenerateToken(user)
            });
        }
    }
}