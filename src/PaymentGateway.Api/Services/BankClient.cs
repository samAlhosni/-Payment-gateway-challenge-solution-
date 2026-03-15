using System.Net;

namespace PaymentGateway.Api.Services;

public class BankClient : IBankClient
{
    private readonly HttpClient _httpClient;

    public BankClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool?> ProcessPaymentAsync(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv)
    {
        var request = new
        {
            card_number = cardNumber,
            expiry_date = $"{expiryMonth:D2}/{expiryYear}",
            currency = currency,
            amount = amount,
            cvv = cvv
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:8080/payments",
            request);

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new Exception("Bank error");

        var result = await response.Content.ReadFromJsonAsync<BankResponse>();

        return result!.Authorized;
    }
}

public class BankResponse
{
    public bool Authorized { get; set; }
    public string Authorization_code { get; set; }
}