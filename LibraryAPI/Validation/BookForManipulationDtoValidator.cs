using FluentValidation;
using LibraryAPI.Models;

namespace LibraryAPI.Validation
{
    public class BookForManipulationDtoValidator : AbstractValidator<BookForManipulationDto>
    {
        public BookForManipulationDtoValidator()
        {
            RuleFor(b => b.Description).NotEqual(b => b.Title)
                .WithMessage("The provided description should be different from the title!");
        }
    }
}
