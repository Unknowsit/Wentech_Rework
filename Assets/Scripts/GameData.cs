using System.Collections.Generic;
using UnityEngine;

public enum OperatorMode
{
    Plus,
    Minus,
    Multiply,
    Divide
}

public enum ParenthesisType
{
    None,
    Open,
    Close,
}

public static class GameData
{
    public static List<OperatorMode> SelectedModes = new List<OperatorMode> { OperatorMode.Plus };
    public static bool UseParentheses = false;

    public static bool IsSingleMode()
    {
        return SelectedModes != null && SelectedModes.Count == 1;
    }

    public static OperatorMode GetSingleMode()
    {
        if (IsSingleMode())
            return SelectedModes[0];

        return OperatorMode.Plus;
    }

    public static OperatorMode GetRandomMode()
    {
        if (SelectedModes == null || SelectedModes.Count == 0)
            return OperatorMode.Plus;

        return SelectedModes[Random.Range(0, SelectedModes.Count)];
    }

    public static bool HasMode(OperatorMode mode)
    {
        return SelectedModes != null && SelectedModes.Contains(mode);
    }

    public static bool ShouldUseParentheses()
    {
        return UseParentheses && !IsSingleMode();
    }
}