using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AadProtectedApi.Models;

namespace AadProtectedApi.Controllers {
    
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase {

        private readonly ILogger<IdentityController> Logger;

        public IdentityController(ILogger<IdentityController> logger) {
            this.Logger = logger;
        }

        [HttpGet]
        public IdentityInfo Get() {

            foreach (var c in User.Claims) {
                this.Logger.LogInformation($"Claim {c.Type} => {c.Value}");
            }

            return new IdentityInfo {
                Date = DateTime.UtcNow,
                Issuer = this.User.Claims.Single(c => c.Type == "iss").Value,
                Audience = this.User.Claims.Single(c => c.Type == "aud").Value,
                Scope = this.User.Claims.Single(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope").Value,
                UserId = this.User.Claims.Single(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value,
                Name = this.User.Claims.Single(c => c.Type == "name").Value,
                PreferredUsername = this.User.Claims.Single(c => c.Type == "preferred_username").Value
            };
        }
    }
}
