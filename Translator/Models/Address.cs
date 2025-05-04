using DynamicTranslator.Attributes;

namespace Translator.Models;

[DisableTranslator]
internal sealed class Address
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}
