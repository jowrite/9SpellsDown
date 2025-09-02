using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager rm; 
    public static MenuManager mm;

    public ElementType foilElement { get; private set; }

    [SerializeField] private AudioClip foilRevealSFX;
    [SerializeField] private TMPro.TMP_Text foilLabel; //instead of text we should do image icons?

    private void Awake()
    {
        if (rm == null) rm = this;
        else Destroy(gameObject);

    }

    public void StartRound()
    {
        RollFoilElement();
        DealCards();
        //PromptWildMagic();
    }

    private void RollFoilElement()
    {
        //Changing to 0-4 to see if it fixes Foil bug
        int roll = Random.Range(0, 4);
        foilElement = (ElementType)roll;

        if (foilLabel != null)
        {
            foilLabel.text = foilElement != ElementType.None
                ? $"Foil Element: {foilElement.ToString()}"
                : "No Foil Element";
        }

        if (AudioManager.am && foilRevealSFX) AudioManager.am.PlaySFX(foilRevealSFX);

        Debug.Log($"Foil element for this round is: {foilElement}");
    }

    private void PromptWildMagic()
    {
        MenuManager.mm.PromptWildMagic();
    }

    private void DealCards()
    {
        DeckManager.dm.ShuffleAndDeal();
    }

}
