using AutoMapper;
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
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public UserController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        [AllowAnonymous]
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {            
            if (await _userService.ExistsAsync(model.Code, model.UserName, model.Email))
            {
                return Ok(new BaseResult<UserModel>
                {
                    Status = false,
                    Message = "Người dùng đã tồn tại"
                });
            }

            var entity = _mapper.Map<User>(model);

            await _userService.InsertAsync(entity, model.Password);

            return Ok(new BaseResult<UserModel>
            {
                Status = true,
                Data = model
            });
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authentication(AuthenticateRequest auth)
        {
            var user = await _userService.Authentication(auth.UserName, auth.Password);

            if (user == null)
            {
                return BadRequest(new BaseResult<UserModel>
                {
                    Status = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                });
            }

            var token = _userService.GenerateJwtToken(user);

            var model = _mapper.Map<UserModel>(user);

            return Ok(new BaseResult<UserModel>
            {
                Status = true,
                Data = model,
                Token = token
            });
        }

        [Route("get-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _userService.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound(new BaseResult<UserModel>
                {
                    Status = false,
                    Message = "Người dùng không tồn tại"
                });
            }

            var model = _mapper.Map<UserModel>(entity);

            return Ok(new BaseResult<UserModel>
            {
                Status = true,
                Data = model
            });
        }
        #endregion

        #region List

        #endregion

        #region Utilities

        #endregion
    }
}
