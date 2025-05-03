using DynamicTranslator.Attributes;

namespace Translator.Models;

[DisableTranslator]
internal sealed class Address
{
    public int Id { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
}
