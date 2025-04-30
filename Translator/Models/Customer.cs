using DynamicTranslator.Attributes;

namespace Translator.Models;

[TranslateAll]
internal sealed class Customer
{
    [IgnoreTranslation]
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string Profession { get; init; }
}
