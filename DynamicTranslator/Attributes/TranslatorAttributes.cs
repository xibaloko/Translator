using System;

namespace DynamicTranslator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TranslateAllAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class TranslateKeysAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class DisableTranslatorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreTranslationAttribute : Attribute { }
}
