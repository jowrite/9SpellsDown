using UnityEngine;

public static class RomanNumerals
{
    private static readonly string[] Numerals =
    {
        "O", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII"
    };

    public static string ToRoman(int number)
    {
        if (number >= 0 && number <= 13)
        {
            return Numerals[number];
        }

        Debug.LogWarning("Number out of range for Roman numeral conversion (1-13).");
        return number.ToString();
    }
}
