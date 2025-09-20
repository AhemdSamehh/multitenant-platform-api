using ABCShared.Library.Models.Requests.Schools;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Validations
{
    public class CreateSchoolRequestValidator : AbstractValidator<CreateSchoolRequest>
    {
        public CreateSchoolRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("School name is required.")
                .MaximumLength(100).WithMessage("School name must not exceed 100 characters.");

            RuleFor(request => request.EstablishedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Established date must be in the past or today.");
        }
    }
}
