using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Contracts;

public interface IParented<T>
{
    [JsonIgnore] T Parent { get; set; }
}