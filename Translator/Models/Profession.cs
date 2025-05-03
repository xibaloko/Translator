using DynamicTranslator.Attributes;

namespace Translator.Models;

internal sealed class Profession
{
    [IgnoreTranslation]
    public int Id { get; init; }
    public required string Name { get; init; }
}
