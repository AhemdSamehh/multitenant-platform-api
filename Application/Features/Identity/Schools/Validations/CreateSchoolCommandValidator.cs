using Application.Features.Identity.Schools.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Validations
{
    public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommands>
    {
        public CreateSchoolCommandValidator()
        {
            RuleFor(command => command.createSchool)
                .SetValidator(new CreateSchoolRequestValidator());
        }
    }
}
