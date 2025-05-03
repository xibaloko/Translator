using DynamicTranslator.Attributes;

namespace Translator.Models;

internal sealed class Customer
{
    [IgnoreTranslation]
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required Address Address { get; init; }
    public List<Profession> Professions { get; init; } = [];
}
