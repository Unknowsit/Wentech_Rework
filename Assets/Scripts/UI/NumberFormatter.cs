using UnityEngine;

public static class NumberFormatter
{
    public static string FormatWithCommas(int number)
    {
        return number.ToString("N0");
    }

    public static string FormatWithCommas(float number, int decimalPlaces = 2)
    {
        if (decimalPlaces == 0)
        {
            return Mathf.RoundToInt(number).ToString("N0");
        }

        string format = "N" + decimalPlaces;
        return number.ToString(format);
    }

    public static string FormatSmart(float number)
    {
        if (Mathf.Approximately(number % 1, 0))
        {
            return FormatWithCommas((int)number);
        }
        else
        {
            return FormatWithCommas(number, 2);
        }
    }
}