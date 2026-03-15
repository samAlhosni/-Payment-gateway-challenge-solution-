using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    private readonly string[] _supportedCurrencies = { "USD", "EUR", "GBP" };

    public PostPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Matches("^[0-9]+$")
            .Length(14, 19)
            .WithMessage("Card number must contain only digits and be between 14 and 19 characters.");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Expiry month must be between 1 and 12.");

        RuleFor(x => x.ExpiryYear)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Expiry year must be in the future.");

        RuleFor(x => x)
            .Must(BeInFuture)
            .WithMessage("Card expiry date must be in the future.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(BeSupportedCurrency)
            .WithMessage("Currency must be one of the supported ISO codes: USD, EUR, GBP.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Cvv)
            .Matches("^[0-9]{3,4}$")
            .WithMessage("CVV must contain 3 or 4 digits.");
    }

    private bool BeInFuture(PostPaymentRequest request)
    {
        if (request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
            return false;
        var expiry = new DateTime(request.ExpiryYear, request.ExpiryMonth, 1)
            .AddMonths(1)
            .AddDays(-1);

        return expiry > DateTime.UtcNow;
    }

    private bool BeSupportedCurrency(string currency)
    {
        return _supportedCurrencies
            .Contains(currency.ToUpper());
    }
}