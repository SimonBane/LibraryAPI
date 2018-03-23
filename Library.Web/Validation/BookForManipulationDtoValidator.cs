using FluentValidation;
using Library.Web.Models;

namespace Library.Web.Validation
{
    internal class BookForManipulationDtoValidator : AbstractValidator<BookForManipulationDto>
    {
        public BookForManipulationDtoValidator()
        {
            RuleFor(b => b.Description).NotEqual(b => b.Title)
                .WithMessage("The provided description should be different from the title!");
        }
    }
}
