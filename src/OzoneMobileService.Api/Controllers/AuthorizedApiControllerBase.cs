using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OzoneMobileService.Api.Controllers;

[Authorize]
public abstract class AuthorizedApiControllerBase : ApiControllerBase;
