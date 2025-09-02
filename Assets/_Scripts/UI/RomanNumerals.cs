using UnityEngine;

public static class RomanNumerals
{
    private static readonly string[] Numerals =
    {
        "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII"
    };

    public static string ToRoman(int number)
    {
        if (number >= 1 && number <= 13)
        {
            return Numerals[number - 1];
        }

        Debug.LogWarning("Number out of range for Roman numeral conversion (1-13).");
        return number.ToString();
    }
}
