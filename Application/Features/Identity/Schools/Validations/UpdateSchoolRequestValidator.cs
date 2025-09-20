using ABCShared.Library.Models.Requests.Schools;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Validations
{
    public class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
    {
        public UpdateSchoolRequestValidator(ISchoolService schoolService)
        {

            RuleFor(req => req.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await schoolService.GetByIdAsync(id) is School schoolInDb && schoolInDb.Id == id)
                .WithMessage("School with the specified ID does not exist.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("School name is required.")
                .MaximumLength(100).WithMessage("School name cannot exceed 100 characters.");
            RuleFor(x => x.EstablishedDate)
                .NotEmpty().WithMessage("Established date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Established date cannot be in the future.");
        }
    }
    
}
