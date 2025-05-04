using Microsoft.AspNetCore.Mvc;
using Translator.Enums;
using Translator.Models;

namespace Translator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private static readonly List<Customer> _customers = [];

    [HttpGet]
    public IActionResult Get()
    {
        SeedCustomers();

        return Ok(_customers);
    }

    private static void SeedCustomers()
    {
        _customers.AddRange([
            new Customer
        {
            Id = 1,
            Name = "Lucas Militão",
            Email = "lucas@example.com",
            Phone = "(11) 91234-5678",
            Address = new Address
            {
                Id = 1,
                Type = "Street",
                Street = "Rua das Palmeiras, 123",
                City = "São Paulo",
                State = "SP"
            },
            Professions =
            [
                new Profession { Id = 1, Name = "Software Engineer" },
                new Profession { Id = 2, Name = "Developer" }
            ],
            CustomerType = CustomerType.Premium,
            Active = true,
            CreatedAt = new DateTime(2023, 5, 10)
        },
        new Customer
        {
            Id = 2,
            Name = "Giovanna Silva",
            Email = "giovanna@example.com",
            Phone = null,
            Address = null,
            Professions = [],
            CustomerType = CustomerType.Basic,
            Active = false,
            CreatedAt = new DateTime(2022, 8, 1)
        },
        new Customer
        {
            Id = 3,
            Name = "Carlos Souza",
            Email = "carlos@example.com",
            Phone = "(31) 98877-1122",
            Address = new Address
            {
                Id = 3,
                Type = "Square",
                Street = "Praça Central, 789",
                City = "Belo Horizonte",
                State = "MG"
            },
            Professions =
            [
                new Profession { Id = 3, Name = "Project Manager" }
            ],
            CustomerType = CustomerType.Vip,
            Active = true,
            CreatedAt = DateTime.UtcNow
        },
        new Customer
        {
            Id = 4,
            Name = "Objeto Genérico",
            Email = "generic@example.com",
            Phone = "000000000",
            Professions = [],
            CustomerType = CustomerType.Basic,
            Active = true,
            CreatedAt = DateTime.UtcNow
        }
        ]);
    }
}
