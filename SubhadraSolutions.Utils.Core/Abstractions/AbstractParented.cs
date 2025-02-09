using SubhadraSolutions.Utils.Contracts;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Abstractions;

public abstract class AbstractParented<T> : IParented<T>
{
    [JsonIgnore] public T Parent { get; set; }
}