using System;
using System.Web.Http.Validation;

namespace xggameplan.Validations
{
    internal sealed class BodyModelValidator : DefaultBodyModelValidator
    {
        public override bool ShouldValidateType(Type type) => type.Namespace != "NodaTime" &&
                    base.ShouldValidateType(type);
    }
}
