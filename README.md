# Payment Gateway API

## Overview

This project implements a simple Payment Gateway API responsible for processing card payments through a simulated bank service.

The API allows merchants to:

- Submit a payment
- Retrieve the status of a previously submitted payment

The bank interaction is simulated using the provided bank simulator.

---

# Design Considerations

## Separation of Responsibilities

The application is structured using a simple layered architecture:

Controller → Service → Bank Client → Repository

- **Controller**
  Handles HTTP requests and responses.

- **Service (PaymentService)**
  Contains the core business logic for processing payments.

- **BankClient**
  Responsible for communicating with the external bank simulator.

- **Repository**
  Stores processed payments in memory.

This separation makes the system easier to test and maintain.

---

## Bank Communication

The `BankClient` encapsulates all communication with the bank simulator using `HttpClient`.

The simulator responses are interpreted as:

| Bank Response | Result |
|---|---|
| 200 Authorized | Payment Authorized |
| 200 Unauthorized | Payment Declined |
| 503 Service Unavailable | Bank unavailable |

If the bank is unavailable, the service throws an exception which can be translated into an appropriate HTTP response.

---

## Security Considerations

To avoid exposing sensitive card information:

- Only the **last four digits of the card number** are stored
- Full card numbers are **never persisted**

This mirrors common industry practices used in payment systems.

---

## Validation

Input validation is implemented using **FluentValidation** to ensure:

- Valid card number format
- Expiry date is valid and in the future
- Amount is provided
- Currency is provided

This prevents invalid requests from reaching the payment processing logic.

---

## Testing Strategy

Unit tests were implemented using:

- **xUnit**
- **FluentAssertions**
- **NSubstitute**

Tests cover:

- Payment processing logic
- Controller responses
- Validation rules

The bank client is mocked in service tests to isolate business logic from external dependencies.

---

# Assumptions

The following assumptions were made during implementation:

- The payment `amount` is provided in **minor currency units** (e.g. 100 = $1.00).
- The bank simulator determines authorization based on the **last digit of the card number**.
- The system does not persist data permanently and instead uses an **in-memory repository** for simplicity.
- Card numbers are provided as strings to preserve leading zeros and match typical payment gateway implementations.

---

# Idempotency (Design Consideration)

In a real payment gateway, idempotency is critical to prevent duplicate charges if a request is retried.

A production implementation would include:

- An **Idempotency Key or CorrelationId**
- Stored with each payment request
- If the same key is received again, the previous result would be returned instead of processing a new payment.

This was discussed but not implemented due to the scope of the exercise.

---

# Possible Improvements

If this were a production system, the following improvements would be implemented:

- Persistent database storage
- Idempotency key support
- Retry policies for bank communication
- Structured logging
- Monitoring and metrics
- Authentication and authorization

---

# Running the Project

Run the bank simulator:

```bash
docker compose up

Run the API:

```bash
dotnet run

Run tests:

```bash
dotnet test