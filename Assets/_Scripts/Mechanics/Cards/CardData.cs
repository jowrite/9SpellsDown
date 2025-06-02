using UnityEngine;
using DG.Tweening;

public enum ElementType { Fire, Water, Earth, Air, None }



[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card")]

public class CardData : ScriptableObject
{
    public string cardName;
    public ElementType element;
    public int value;
    public Sprite elementIcon; // Uncomment when card art ready
}
