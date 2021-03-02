using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BRS;
using xggameplan.Filters;
using xggameplan.model.External;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    [RoutePrefix("brs-templates")]
    public class BRSTemplatesController : ApiController
    {
        private readonly IBRSConfigurationTemplateRepository _repository;
        private readonly IModelDataValidator<CreateOrUpdateBRSConfigurationTemplateModel> _validator;
        private readonly IMapper _mapper;

        public BRSTemplatesController(
            IBRSConfigurationTemplateRepository repository,
            IModelDataValidator<CreateOrUpdateBRSConfigurationTemplateModel> validator,
            IMapper mapper)
        { 
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        /// <summary>
        /// Create BRS template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("brs-templates")]
        [ResponseType(typeof(BRSConfigurationTemplateModel))]
        public IHttpActionResult Post(CreateOrUpdateBRSConfigurationTemplateModel model)
        {
            if (!_validator.IsValid(model))
            {
                return _validator.BadRequest();
            }

            var item = _mapper.Map<BRSConfigurationTemplate>(model);
            item.LastModified = DateTime.UtcNow;

            _repository.Add(item);
            _repository.SaveChanges();

            return Ok(_mapper.Map<BRSConfigurationTemplateModel>(item));
        }

        /// <summary>
        /// Update BRS template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("brs-templates")]
        [ResponseType(typeof(BRSConfigurationTemplateModel))]
        public IHttpActionResult Put(int id, CreateOrUpdateBRSConfigurationTemplateModel model)
        {
            if (model != null && id != model.Id)
            {
                ModelState.AddModelError(nameof(BRSConfigurationTemplateModel.Id), "Id does not match");
                return BadRequest(ModelState);
            }

            if (!_validator.IsValid(model))
            {
                return _validator.BadRequest();
            }

            var entity = _repository.Get(model.Id);

            if (entity == null)
            {
                return NotFound();
            }

            var item = _mapper.Map<BRSConfigurationTemplate>(model);
            entity.Name = item.Name;
            entity.LastModified = DateTime.UtcNow;
            entity.KPIConfigurations = item.KPIConfigurations;

            _repository.Update(entity);
            _repository.SaveChanges();

            return Ok(_mapper.Map<BRSConfigurationTemplateModel>(entity));
        }

        /// <summary>
        /// Delete BRS template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("brs-templates")]
        public IHttpActionResult Delete(int id)
        {
            var item = _repository.Get(id);
            if (item is null)
            {
                return NotFound();
            }

            if (item.IsDefault)
            {
                return BadRequest("You can't delete default BRS Configuration template");
            }

            _repository.Delete(id);
            _repository.SaveChanges();

            return this.NoContent();
        }

        /// <summary>
        /// Get BRS template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("brs-templates")]
        [ResponseType(typeof(BRSConfigurationTemplateModel))]
        public IHttpActionResult Get(int id)
        {
            var item = _repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BRSConfigurationTemplateModel>(item));
        }

        /// <summary>
        ///  Get all BRS templates
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("brs-templates")]
        [ResponseType(typeof(IEnumerable<BRSConfigurationTemplateModel>))]
        public IHttpActionResult GetAll()
        {
            var items = _repository.GetAll();
            return Ok(_mapper.Map<List<BRSConfigurationTemplateModel>>(items));
        }

        /// <summary>
        /// Set BRS template as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("default/{id}")]
        [AuthorizeRequest("brs-templates")]
        public IHttpActionResult PutDefault(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }

            if (_repository.ChangeDefaultConfiguration(id))
            {
                _repository.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
