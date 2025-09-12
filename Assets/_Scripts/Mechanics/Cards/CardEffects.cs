using TMPro;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    public static CardEffects cardEffects;

    [Header("UI Refs")]
    [SerializeField] CanvasGroup warningDisplay;
    [SerializeField] TextMeshProUGUI warningText;

    [Header("SFX")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip badPlayClip;
    //[SerializeField] AUdioClip winTurnClip;

    [Header("Bad Play Effects")]
    //[SerializeField] float badPlayShakeDuration = 0.5f;
    [SerializeField] Color goodPlay = Color.white;
    [SerializeField] Color badPlayColor = new Color(1f, 1f, 1f, 0.5f);

    private void Awake()
    {
        if (cardEffects == null) cardEffects = this;
        else Destroy(this);

        if (warningDisplay != null)
        {
            warningDisplay.alpha = 0f;
            warningDisplay.gameObject.SetActive(false);
        }
    }

    public void ShowWarning(string message, Transform illegalCard = null, float duration = 2f)
    {
        if (warningDisplay != null && warningText != null)
        {
            warningText.text = message;
            warningDisplay.gameObject.SetActive(true);
            warningDisplay.alpha = 1f;

            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), duration);

        }

        PlaySFX(badPlayClip);
        
        if (illegalCard != null)
        {
            ShakeCard(illegalCard);
        }
    }

    private void HideWarning()
    {
        if (warningDisplay != null)
        {
            warningDisplay.alpha = 0f;
            warningDisplay.gameObject.SetActive(false);
        }
    }

    private void PlaySFX(AudioClip clip)
    {

    }

    private void ShakeCard(Transform cardTransform)
    {

    }

}
