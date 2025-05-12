using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager rm; 

    public ElementType foilElement { get; private set; }

    [SerializeField] private AudioClip foilRevealSFX;
    [SerializeField] private TMPro.TMP_Text foilLabel;

    private void Awake()
    {
        if (rm == null) rm = this;
        else Destroy(gameObject);
    }

    public void StartRound()
    {
        RollFoilElement();
        DealCards();
        PromptWildMagic();
    }

    private void RollFoilElement()
    {
        int roll = Random.Range(0, 5);
        foilElement = (ElementType)roll;

        if (foilLabel != null)
        {
            foilLabel.text = foilElement != ElementType.None
                ? $"Foil Element: {foilElement.ToString()}"
                : "No Foil Element";
        }

        //if (AudioManager.instance && foilRevealSFX)
        //    AudioManager.instance.PlaySFX(foilRevealSFX);

        Debug.Log($"Foil element for this round is: {foilElement}");
    }

    private void DealCards()
    {
        DeckManager.dm.ShuffleAndDeal();
    }

    private void PromptWildMagic()
    {
        //Show UI for the wild magic decision (player, then AI)
    }

}
