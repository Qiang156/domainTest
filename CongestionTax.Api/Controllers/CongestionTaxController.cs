using CongestionTax.Api.Models;
using CongestionTax.Business.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CongestionTax.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CongestionTaxController : ControllerBase
    {
        private readonly ILogger<CongestionTaxController> _logger;
        private readonly IMediator _mediator;

        public CongestionTaxController(ILogger<CongestionTaxController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry([FromBody] AddEntryRequest request)
        {
            await _mediator.Send(new AddEntryCommand(request.RegistrationNumber, request.Vehicle, request.City));

            return Ok();
        }
    }
}