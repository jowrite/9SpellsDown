using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material leaderMaterial;

    public void SetLeaderHighlight(bool isLeader)
    {
        if (frame == null || defaultMaterial == null || leaderMaterial == null) return;

        frame.material = isLeader ? leaderMaterial : defaultMaterial;
    }
}
