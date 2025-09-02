using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player References")]
    private PlayerData playerData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI curseLevelText;
    [SerializeField] private TextMeshProUGUI spellCastText;

    [SerializeField] private Image frame;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material leaderMaterial;

    public void UpdateHUD(PlayerData data)
    {
        playerNameText.text = data.playerName;
        curseLevelText.text = $"{data.curseLevel}";
        spellCastText.text = $"{data.spellCastsThisRound}";
    }

    public void SetLeaderHighlight(bool isLeader)
    {
        if (frame == null || defaultMaterial == null || leaderMaterial == null) return;

        frame.material = isLeader ? leaderMaterial : defaultMaterial;
    }
}
