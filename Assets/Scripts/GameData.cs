using System.Collections.Generic;
using UnityEngine;

public enum OperatorMode
{
    Add,
    Minus,
    Multiply,
    Divide
}

public static class GameData
{
    public static List<OperatorMode> SelectedModes = new List<OperatorMode> { OperatorMode.Add , OperatorMode.Divide };

    public static bool IsSingleMode()
    {
        return SelectedModes != null && SelectedModes.Count == 1;
    }

    public static OperatorMode GetSingleMode()
    {
        if (IsSingleMode())
            return SelectedModes[0];

        return OperatorMode.Add;
    }

    public static OperatorMode GetRandomMode()
    {
        if (SelectedModes == null || SelectedModes.Count == 0)
            return OperatorMode.Add;

        return SelectedModes[Random.Range(0, SelectedModes.Count)];
    }

    public static bool HasMode(OperatorMode mode)
    {
        return SelectedModes != null && SelectedModes.Contains(mode);
    }
}