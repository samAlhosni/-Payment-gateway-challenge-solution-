using FluentAssertions;

using NSubstitute;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentServiceTests
{
    private readonly PaymentsRepository _repository;
    private readonly IBankClient _bankClient;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _repository = new PaymentsRepository();
        _bankClient = Substitute.For<IBankClient>();
        _service = new PaymentService(_repository, _bankClient);
    }

    [Theory]
    [InlineData("4111111111111111", true, PaymentStatus.Authorized)]
    [InlineData("4111111111111112", false, PaymentStatus.Declined)]
    public async Task ProcessPayment_ReturnsCorrectStatus(
        string cardNumber,
        bool bankResult,
        PaymentStatus expectedStatus)
    {
        _bankClient.ProcessPaymentAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>())
        .Returns(bankResult);

        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "usd",
            Amount = 100,
            Cvv = "123"
        };

        var result = await _service.ProcessPaymentAsync(request);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task ProcessPayment_ThrowsException_WhenBankUnavailable()
    {
        _bankClient.ProcessPaymentAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>())
        .Returns((bool?)null);

        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111110",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "USD",
            Amount = 100,
            Cvv = "123"
        };

        Func<Task> act = async () => await _service.ProcessPaymentAsync(request);

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Bank unavailable");
    }

    [Theory]
    [InlineData("4111111111111111", "1111")]
    [InlineData("5555444433332222", "2222")]
    [InlineData("1234567812345678", "5678")]
    public async Task ProcessPayment_ExtractsLastFourDigitsCorrectly(
        string cardNumber,
        string expectedLastFour)
    {
        _bankClient.ProcessPaymentAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>())
        .Returns(true);

        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 1,
            ExpiryYear = 2029,
            Currency = "usd",
            Amount = 500,
            Cvv = "123"
        };

        var result = await _service.ProcessPaymentAsync(request);

        result.CardNumberLastFour.Should().Be(expectedLastFour);
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("Usd")]
    [InlineData("USD")]
    public async Task ProcessPayment_NormalizesCurrencyToUpper(string currency)
    {
        _bankClient.ProcessPaymentAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>())
        .Returns(true);

        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 10,
            ExpiryYear = 2028,
            Currency = currency,
            Amount = 100,
            Cvv = "123"
        };

        var result = await _service.ProcessPaymentAsync(request);

        result.Currency.Should().Be("USD");
    }
}