using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Text playerNameText;
    public Text curseLevelText;
    public Text spellCastText;

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
