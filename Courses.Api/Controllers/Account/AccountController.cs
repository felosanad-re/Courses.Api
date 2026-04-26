using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO;
using Courses.Core.ModelsDTO.ResponseDTO;
using Courses.Core.Services.Contract.AccountServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Account
{
    public class AccountController : BaseController
    {
        #region Inject Services
        protected readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        #endregion

        #region Register
        [HttpPost("Register")] // POST: /api/Account/Register
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApplicationServiceResult<CreateAccountResponse>>> Register([FromBody] CreateAccountRequest req)
        {
            var result = await _accountService.RegisterAsync(req);
            if (!result.Succeed)
            {
                if (result.Message?.Contains("Already Exist", StringComparison.OrdinalIgnoreCase) == true) 
                {
                    return Conflict(new {message = result.Message, error = result.Errors});
                };

                return BadRequest(new { message = result.Message, errors = result.Errors });
            }
            return Ok(result);
        }
        #endregion

        #region Confirm
        [HttpGet("Confirm")] // GET: /api/Account/Confirm
        public async Task<ActionResult<ApplicationServiceResult<ConfirmAccountResponse>>> Confirm([FromQuery]ConfirmAccountRequest request)
        {
            var result = await _accountService.ConfirmAccount(request);
            if (!result.Succeed)
                return BadRequest(result.Message);

            return Ok(result);
        }
        #endregion
    }
}
