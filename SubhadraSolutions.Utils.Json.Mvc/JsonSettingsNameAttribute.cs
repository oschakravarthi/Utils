using System;

namespace SubhadraSolutions.Utils.Json.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JsonSettingsNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}