using Microsoft.AspNetCore.Mvc;
using TSDC.ApiHelper;
using TSDC.Core.Domain.Master;
using TSDC.Service.Master;
using TSDC.SharedMvc.Master.Models;
using TSDC.Web.Framework;

namespace TSDC.Api.Master.Controllers
{
    [Authorize]
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Fields
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public UserController(
            IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Methods
        [AllowAnonymous]
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (await _userService.ExistsAsync(user.Code, user.UserName, user.Email))
            {
                return Ok(new BaseResult<User>
                {
                    Status = false,
                    Message = "Người dùng đã tồn tại"
                });
            }

            await _userService.InsertAsync(user, user.Password);

            return Ok(new BaseResult<int>
            {
                Status = true,
                Data = user.Id
            });
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authentication(AuthenticateRequest auth)
        {
            var token = await _userService.Authentication(auth.UserName, auth.Password);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new BaseResult<User>
                {
                    Status = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                });
            }

            return Ok(new BaseResult<string>
            {
                Status = true,
                Data = token
            });
        }

        [Route("get-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _userService.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound(new BaseResult<User>
                {
                    Status = false,
                    Message = "Người dùng không tồn tại"
                });
            }

            return Ok(new BaseResult<User>
            {
                Status = true,
                Data = entity
            });
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
