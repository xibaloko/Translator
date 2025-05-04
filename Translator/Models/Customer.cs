using DynamicTranslator.Attributes;
using Translator.Enums;

namespace Translator.Models;

internal sealed class Customer
{
    [IgnoreTranslation]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }
    public List<Profession> Professions { get; set; } = [];
    public CustomerType CustomerType { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}
