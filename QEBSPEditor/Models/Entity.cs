using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace QEBSPEditor.Models;

public class Entity
{
    private Dictionary<string, string> _keyValues { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

    public Dictionary<string, string> KeyValues => _keyValues;
    public EntitySourceHint? SourceHint { get; set; } = null;

    public string? Classname { get => GetKeyValue("classname"); set => SetKeyValue("classname", value); }
    public string? Targetname { get => GetKeyValue("targetname"); set => SetKeyValue("targetname", value); }
    public Vector3? Origin { get => GetKeyValueVector("origin"); set => SetKeyValue("origin", value); }

    public bool HasKeyValue(string key) => _keyValues.ContainsKey(key);
    public void SetKeyValue(string key, string? value)
    {
        if (value is null)
        {
            _keyValues.Remove(key);
            return;
        }

        _keyValues[key] = value;
    }
    public void SetKeyValue(string key, Vector3? value) => SetKeyValue(key, value.HasValue ? FormattableString.Invariant($"{value.Value.X} {value.Value.Y} {value.Value.Z}") : null);

    public string? GetKeyValue(string key, string? defaultValue = null) => _keyValues.TryGetValue(key, out var value) ? value : defaultValue;
    public Vector3? GetKeyValueVector(string key, Vector3? defaultValue = null)
    {
        if (!TryGetKeyValue(key, out var value))
            return defaultValue;

        var split = value.Split(" ");

        if (!float.TryParse(split[0], CultureInfo.InvariantCulture, out var x)) return defaultValue;
        if (!float.TryParse(split[1], CultureInfo.InvariantCulture, out var y)) return defaultValue;
        if (!float.TryParse(split[2], CultureInfo.InvariantCulture, out var z)) return defaultValue;

        return new Vector3(x, y, z);
    }

    public bool TryGetKeyValue(string key, [NotNullWhen(true)] out string? value) => (value = GetKeyValue(key)) != null;
    public bool TryGetKeyValueVector(string key, [NotNullWhen(true)] out Vector3? value) => (value = GetKeyValueVector(key)).HasValue;


    public override int GetHashCode() => HashCode.Combine(KeyValues, SourceHint);
}
