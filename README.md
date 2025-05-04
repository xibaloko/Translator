# DynamicTranslator

DynamicTranslator is a lightweight SDK for automatic response translation in ASP.NET Core APIs using attributes and `IStringLocalizer`.  
Easily localize API responses without writing repetitive localization logic.

---

## 📦 Installation

```bash
dotnet add package DynamicTranslator
```

---

## ⚙️ Configuration

In your `Program.cs`, add the following:

```csharp
// Localization resources configuration
builder.Services.AddDynamicTranslator(options =>
{
    options.ResourceType = typeof(Resources); // Your default .resx resource
});

// Register translation filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GenericTranslationFilter>();
});
```

---

## 📁 Localization folder structure

Create a folder called `Localization/` in the **root** of your project and add your `.resx` files:

```
/YourProject
│
├── Localization/
│   ├── Resources.resx              // Default (e.g., pt-BR)
│   ├── Resources.en.resx           // English
│   ├── Resources.es.resx           // Spanish
│   └── Resources.fr.resx           // French
```

The `Resources` file must contain the translated keys used in your models or attributes.  
Each `.resx` file should follow the [ResourceManager naming convention](https://learn.microsoft.com/en-us/dotnet/core/extensions/work-with-resx-files).

---

## 🏷️ Attributes

Use the following attributes in your response models:

- `[TranslateAll]` → Translates all string properties
- `[TranslateKeys("key1", "key2")]` → Translates only specific properties
- `[IgnoreTranslation]` → Skips a specific property
- `[DisableTranslator]` → Skips the entire object

Example:

```csharp
[TranslateAll]
public class CustomerResponse
{
    public string Name { get; set; }

    [IgnoreTranslation]
    public string Email { get; set; }

    public AddressResponse Address { get; set; }
}
```

---

## 🌐 Accept-Language header

Translation will automatically be applied based on the `Accept-Language` header sent in the request.

Example:

```http
GET /api/customer
Accept-Language: es
```

---

## ✅ Example response before/after

**Input (`pt-BR`):**

```json
{
  "name": "Lucas Militão",
  "address": {
    "city": "São Paulo"
  }
}
```

**Translated (`Accept-Language: en`):**

```json
{
  "name": "Lucas Militão",
  "address": {
    "city": "São Paulo" // Assuming not found in .resx, fallback to original
  }
}
```

---

## 📚 License

MIT © [Ulisses Inácio](https://github.com/xibaloko)