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
    [Route("organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        #region Fields
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public OrganizationController(
            IOrganizationService organizationService,
            IMapper mapper)
        {
            _organizationService = organizationService;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(OrganizationModel model)
        {
            if (await _organizationService.ExistsAsync(model.Code))
            {
                return BadRequest(new BaseResult<object>
                {
                    Status = false,
                    Message = "Đơn vị đã tồn tại"
                });
            }

            var entity = _mapper.Map<Organization>(model);
            await _organizationService.InsertAsync(entity);

            return Ok(new BaseResult<int>
            {
                Status = true,
                Data = entity.Id,
                Message = "Thêm mới thành công"
            });
        }
        #endregion

        #region List
        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] OrganizationSearchModel searchModel)
        {
            var searchContext = new OrganizationSearchContext
            {
                Keywords = searchModel.Keywords,
                PageNumber = searchModel.PageNumber,
                PageSize = searchModel.PageSize,
                Status = (int)searchModel.Status
            };

            var organizations = _organizationService.Get(searchContext);

            var models = new List<OrganizationModel>();

            if (organizations?.Count > 0)
            {
                foreach(var e in organizations)
                {
                    var m = _mapper.Map<OrganizationModel>(e);
                    models.Add(m);
                }
            }

            return Ok(new BaseResult<List<OrganizationModel>>
            {
                Status = true,
                Data = models,
                TotalCount = organizations?.Count ?? 0
            });
        }
        #endregion

        #region Utilities

        #endregion
    }
}
