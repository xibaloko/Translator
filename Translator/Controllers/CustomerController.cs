using Microsoft.AspNetCore.Mvc;
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
        _customers.AddRange(
        [
            new Customer
        {
            Id = 1,
            Name = "Lucas Militão",
            Email = "lucas@example.com",
            Phone = "(11) 91234-5678",
            Address = new Address
            {
                Id = 1,
                Street = "Rua das Palmeiras, 123",
                City = "São Paulo",
                State = "SP"
            },
            Professions =
            [
                new Profession { Id = 1, Name = "Software Engineer" },
                new Profession { Id = 2, Name = "Developer" }
            ]
        },
        new Customer
        {
            Id = 2,
            Name = "Giovanna Silva",
            Email = "giovanna@example.com",
            Phone = "(21) 99876-5432",
            Address = new Address
            {
                Id = 2,
                Street = "Avenida Brasil, 456",
                City = "Rio de Janeiro",
                State = "RJ"
            },
            Professions =
            [
                new Profession { Id = 2, Name = "Graphic Designer" }
            ]
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
                Street = "Praça Central, 789",
                City = "Belo Horizonte",
                State = "MG"
            },
            Professions =
            [
                new Profession { Id = 3, Name = "Project Manager" }
            ]
        }
        ]);
    }

}
