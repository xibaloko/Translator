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

- `[IgnoreTranslation]` → Skips a specific property
- `[DisableTranslator]` → Skips the entire object

Example:

```csharp
public class ProductResponse
{
    [IgnoreTranslation]
    public string Name { get; set; }

    public string Type { get; set; }
}
```

---

## 🌐 Accept-Language header

Translation will automatically be applied based on the `Accept-Language` header sent in the request.

Example:

```http
GET /api/products
Accept-Language: pt-BR
```

---

## ✅ Example response before/after

**Input (`en-US`):**

```json
{
  "name": "Tesla Model Y",
  "type": "Electric Vehicle"
}
```

**Translated (`Accept-Language: pt-BR`):**

```json
{
  "name": "Tesla Model Y",
  "type": "Veículo Elétrico"
}
```

---

## 📚 License

MIT © [Ulisses Inácio](https://github.com/xibaloko)
