namespace QEBSPEditor.Core.Utilities;

public static class MathUtils
{
    public static int Max(int value1, params int[] values)
    {
        int max = value1;
        for(var i=0;i<values.Length; i++)
            max = Math.Max(max, values[i]);
        return max;
    }
}
