using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Results;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using xggameplan.model.External;

namespace xggameplan.Validations
{
    public abstract class ModelDataValidatorBase<T> : IModelDataValidator<T>
        where T : class
    {
        private IValidator<T> _validator;
        private IEnumerable<ValidationResult> _validationResults;
        private readonly JsonMediaTypeFormatter _jsonMediaTypeFormatter = new JsonMediaTypeFormatter()
        {
            SerializerSettings = new JsonSerializerSettings()
            { ContractResolver = new CamelCasePropertyNamesContractResolver() }
        };

        public ModelDataValidatorBase(IValidator<T> validator)
        {
            _validator = validator;
        }

        public IEnumerable<ErrorModel> Errors => GetErrors();

        public virtual bool IsValid(T model)
        {
            _validationResults = new ValidationResult[] { Validate(model) };
            return _validationResults.FirstOrDefault().IsValid;
        }

        public virtual bool IsValid(IEnumerable<T> model)
        {
            _validationResults = Validate(model);           

            return _validationResults?.All(r => r.IsValid) == true;
        }

        public ValidationResult Validate(T model)
        {
            return model != null ? _validator.Validate(model) :
                   new ValidationResult(new[] { new ValidationFailure($"Model Error", "Model Should not be null") });
        }

        public IEnumerable<ValidationResult> Validate(IEnumerable<T> models)
        {
            return models?.Select(m => Validate(m));
        }      

        public override string ToString()
        {
            return ToJson();
        }

        public string ToJson()
        {
            var errors = GetErrors();
            return (errors != null) ? JsonConvert.SerializeObject(errors, _jsonMediaTypeFormatter.SerializerSettings) : string.Empty;
        }

        private List<ErrorModel> GetErrors()
        {
            if (_validationResults == null || _validationResults.All(r => r.IsValid)) { return null; }

            return _validationResults.Where(r => !r.IsValid)
                                     .SelectMany(r => r.Errors)
                                     .Select(a => new ErrorModel
                                     {
                                         ErrorField = a.PropertyName,
                                         ErrorMessage = a.ErrorMessage,
                                         ErrorCode = a.PropertyName // The ErrorCode currently uses property names
                                     }).ToList();           
        }

        public IHttpActionResult BadRequest()
        {  
            return BadRequest(Errors);
        }

        public IHttpActionResult BadRequest(ErrorModel error)
        {
            return BadRequest(error != null ? new ErrorModel[] { error } : null);
        }        

        public IHttpActionResult BadRequest(IEnumerable<ErrorModel> errors)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (errors != null && errors.Any(e => e != null))
            {
                response.Content = new ObjectContent<IEnumerable<ErrorModel>>(errors.Where(e => e != null),
                                                                              _jsonMediaTypeFormatter);
            }
            return new ResponseMessageResult(response);
        }       
    }
}
