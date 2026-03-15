using FluentAssertions;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Validators;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator = new();

    [Theory]
    [InlineData("", false)]
    [InlineData("123", false)]
    [InlineData("4111111111111111", true)]
    public void CardNumberValidation(string cardNumber, bool expectedValid)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 10,
            ExpiryYear = 2028,
            Currency = "USD",
            Amount = 100,
            Cvv = "123"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(12, true)]
    [InlineData(13, false)]
    public void ExpiryMonthValidation(int month, bool expectedValid)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = month,
            ExpiryYear = 2028,
            Currency = "USD",
            Amount = 100,
            Cvv = "123"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("usd", true)]
    [InlineData("USD", true)]
    public void CurrencyValidation(string currency, bool expectedValid)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 10,
            ExpiryYear = 2028,
            Currency = currency,
            Amount = 100,
            Cvv = "123"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(1000, true)]
    public void AmountValidation(int amount, bool expectedValid)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 10,
            ExpiryYear = 2028,
            Currency = "USD",
            Amount = amount,
            Cvv = "123"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedValid);
    }
}