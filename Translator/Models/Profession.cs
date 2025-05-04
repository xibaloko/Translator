using DynamicTranslator.Attributes;

namespace Translator.Models;

internal sealed class Profession
{
    [IgnoreTranslation]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
