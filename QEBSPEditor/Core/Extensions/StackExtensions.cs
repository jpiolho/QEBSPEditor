namespace QEBSPEditor.Core.Extensions;

public static class StackExtensions
{
    public static IReadOnlyList<TItem> PopUntil<TItem>(this Stack<TItem> stack, TItem item)
    {
        var _pops = new List<TItem>();
        while (!EqualityComparer<TItem>.Default.Equals(stack.Peek(), item) && stack.Count > 0)
            _pops.Add(stack.Pop());

        return _pops;
    }
}
