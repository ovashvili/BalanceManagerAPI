using CasinoProject.Application.BalanceManager.Queries.DepositToCasino;
using CasinoProject.Application.BalanceManager.Queries.GetCasinoBalance;
using CasinoProject.Application.BalanceManager.Queries.WithdrawFromCasino;
using CasinoProject.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoProject.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api")]
    public class BalanceManagerController : ApiControllerBase
    {
        public BalanceManagerController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get casino's balance
        /// </summary>
        [HttpGet("/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetCasinoBalanceVM>> GetCasinoBalanceAsync(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetCasinoBalanceQuery{ }, cancellationToken));
        }

        /// <summary>
        /// Withdraw from casino
        /// </summary>
        /// <remarks>
        /// <strong>Request params:</strong>
        ///
        /// 1. transactionId is an identifier of the transaction
        /// </remarks>
        [HttpGet("/withdraw/{transactionId}/{amount}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WithdrawFromCasinoVM>> WithdrawFromCasinoAsync([FromRoute] string transactionId, [FromRoute] decimal amount, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new WithdrawFromCasinoQuery
            { 
                TransactionId = transactionId, 
                Amount = amount 
            }, cancellationToken));
        }

        /// <summary>
        /// Deposit to casino
        /// </summary>
        /// <remarks>
        /// <strong>Request params:</strong>
        ///
        /// 1. transactionId is an identifier of the transaction
        /// </remarks>
        [HttpGet("/deposit/{transactionId}/{amount}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DepositToCasinoVM>> DepoistToCasinoAsync([FromRoute] string transactionId, [FromRoute] decimal amount, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DepositToCasinoQuery 
            { 
                TransactionId = transactionId, 
                Amount = amount 
            }, cancellationToken));
        }
    }
}
