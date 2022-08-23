using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CasinoProject.Common
{
    [ApiController]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator Mediator;
        public ApiControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
