namespace PaymentGateway.Api.Services;

public interface IBankClient
{
    Task<bool?> ProcessPaymentAsync(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv);
}