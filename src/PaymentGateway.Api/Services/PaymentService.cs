using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentService
{
    private readonly PaymentsRepository _repository;
    private readonly IBankClient _bankClient;

    public PaymentService(
        PaymentsRepository repository,
        IBankClient bankClient)
    {
        _repository = repository;
        _bankClient = bankClient;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(
        PostPaymentRequest request)
    {
        var bankResult = await _bankClient.ProcessPaymentAsync(
            request.CardNumber,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.Currency.ToUpper(),
            request.Amount,
            request.Cvv);

        if (bankResult == null)
        {
            throw new HttpRequestException("Bank unavailable");
        }

        var status = bankResult.Value
            ? PaymentStatus.Authorized
            : PaymentStatus.Declined;

        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = status,
            CardNumberLastFour = request.CardNumber[^4..],
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency.ToUpper(),
            Amount = request.Amount
        };

        _repository.Add(payment);

        return payment;
    }
}