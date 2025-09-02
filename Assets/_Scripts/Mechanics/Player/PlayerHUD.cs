using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerData playerData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI curseLevelText;
    [SerializeField] private TextMeshProUGUI spellCastText;

    [SerializeField] private Image frame;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material leaderMaterial;

    private void Start()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoresUpdated += UpdateHUD;
        }
        UpdateHUD();
    }

    private void OnDestroy()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoresUpdated -= UpdateHUD;
        }
    }


    public void UpdateHUD()
    {
        if (playerData == null) return;

        if (curseLevelText != null)
        {
            curseLevelText.text = playerData.curseLevel.ToString();
        }

        //Update spell casts
        if (spellCastText != null)
        {
            string currentSpells = RomanNumerals.ToRoman(playerData.spellCastsThisRound);
        }
    }

    public void SetLeaderHighlight(bool isLeader)
    {
        if (frame == null || defaultMaterial == null || leaderMaterial == null) return;
        frame.material = isLeader ? leaderMaterial : defaultMaterial;
    }

    public void OnTrickWon()
    {
        UpdateHUD();

        // Optional: Add a nice animation when a trick is won
        if (spellCastText != null)
        {
            spellCastText.transform.DOScale(1.2f, 0.1f)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}
