using UnityEngine;
using UnityEditor;
using System.IO;

public class CardAssetGenerator
{
    [MenuItem("Tools/Generate CardData Assets")]
    public static void GenerateCards()
    {
        string folderPath = "Assets/Resources/Cards";

        if (Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string[] elements = { "Fire", "Water", "Earth", "Air" };

        foreach (string element in elements)
        {
            for (int value = 1; value <= 13; value++)
            {
                CardData newCard = ScriptableObject.CreateInstance<CardData>();
                newCard.cardName = $"{value} of {element}";
                newCard.element = (ElementType)System.Enum.Parse(typeof(ElementType), element);
                newCard.value = value;

                string assetPath = $"{folderPath}/Card_{element}_{value}.asset";
                AssetDatabase.CreateAsset(newCard, assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CardData assets generated successfully!");
    }
}
