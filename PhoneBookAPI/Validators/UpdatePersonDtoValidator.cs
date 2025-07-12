using FluentValidation;
using PhonebookApi.Dtos;

namespace PhonebookApi.Validators
{
    public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
    {
        public UpdatePersonDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("İsim boş olamaz");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon boş olamaz");
        }
    }
}