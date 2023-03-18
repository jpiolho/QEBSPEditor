using System.Text.RegularExpressions;

namespace QEBSPEditor.Core.EntParsing;

public class RegexBucket
{
    public static RegexBucket Default { get; } = new RegexBucket();

    private Dictionary<string, Regex> _bucket;
    private string? _input;

    public RegexBucket()
    {
        _bucket = new Dictionary<string, Regex>();
        _input = null;
    }

    public RegexBucket(string input) : this()
    {
        _input = input;
    }

    public Match Match(string pattern, int offset, RegexOptions options = RegexOptions.None) => Match(_input!, pattern, offset, options);
    public Match Match(string input, string pattern, int offset, RegexOptions options = RegexOptions.None)
    {
        if (!_bucket.TryGetValue(pattern, out var regex))
            _bucket.Add(pattern, regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline | options));

        return regex.Match(input, offset);
    }
}
