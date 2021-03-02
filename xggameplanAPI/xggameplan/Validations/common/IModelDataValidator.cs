using System.Collections.Generic;
using System.Web.Http;
using FluentValidation.Results;
using xggameplan.model.External;

namespace xggameplan.Validations
{
    public interface IModelDataValidator<T> where T : class
    {
        bool IsValid(T model);

        bool IsValid(IEnumerable<T> model);

        ValidationResult Validate(T model);        

        IEnumerable<ValidationResult> Validate(IEnumerable<T> model);

        IEnumerable<ErrorModel> Errors { get; }

        string ToJson();

        IHttpActionResult BadRequest();

        IHttpActionResult BadRequest(ErrorModel error);

        IHttpActionResult BadRequest(IEnumerable<ErrorModel> errors);
    }
}
