using FluentValidation;
using PhonebookApi.Dtos;

namespace PhonebookApi.Validators
{
    public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
    {
        public CreatePersonDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("İsim boş olamaz")
                .MaximumLength(100).WithMessage("İsim 100 karakterden uzun olamaz");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur")
                .Matches(@"^\d{10,11}$").WithMessage("Geçerli bir telefon numarası girin (sadece rakam)");
        }
    }
}