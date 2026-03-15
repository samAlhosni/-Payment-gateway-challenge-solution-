using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    PaymentService paymentService,
    PaymentsRepository paymentsRepository) : Controller
{
    private readonly PaymentService _paymentService = paymentService;
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPayment(
    PostPaymentRequest request)
    {
        try
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            return Ok(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, "Bank unavailable");
        }
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GetPaymentResponse?> GetPayment(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }
}