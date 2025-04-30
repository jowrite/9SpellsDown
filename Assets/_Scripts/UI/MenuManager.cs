using UnityEngine;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonPressSFX;

    [SerializeField] private Animator anim;

    [SerializeField] private TextMeshProUGUI ButtonText;

    //Panel visibility vars
    private bool isRulesPanelOpen = false;
    private bool isScoresPanelOpen = false;
    private bool isUpgradesPanelOpen = false;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //if (AudioManager.instance) AudioManager.instance.PlayMusic(menuMusic);
    }

    //Taps will toggle the panel visibility
    public void ToggleRules()
    {
        isRulesPanelOpen = !isRulesPanelOpen;
        //if(AudioManager.instance) AudioManager.instance.PlaySFX(buttonPressSFX);

        if (isRulesPanelOpen)
        {
            anim.Play("ShowRulesPanel");
            SetTextVisible(ButtonText, true);
        }
        else
        {
            anim.Play("HideRulesPanel");
            SetTextVisible(ButtonText, false);
        }
    }

    private void SetTextVisible(TextMeshProUGUI rulesButtonText, bool v)
    {
        Color color = rulesButtonText.color;
        color.a = v ? 1 : 0; //Alpha 1 = vivisble, 0 = invisible
        rulesButtonText.color = color;
    }

    public void ToggleScores()
    {
        isScoresPanelOpen = !isScoresPanelOpen;
        //if(AudioManager.instance) AudioManager.instance.PlaySFX(buttonPressSFX);

        if (isScoresPanelOpen)
        {
            anim.Play("ShowScores");
            SetTextVisible(ButtonText, true);
        }    
        else
        {
            anim.Play("HideScores");
            SetTextVisible(ButtonText, false);
        }
    }
    public void ToggleUpgrades()
    {
        isUpgradesPanelOpen = !isUpgradesPanelOpen;
        //if(AudioManager.instance) AudioManager.instance.PlaySFX(buttonPressSFX);

        if (isUpgradesPanelOpen)
        {
            anim.Play("ShowUpgrades");
            SetTextVisible(ButtonText, true);
        }
        else
        {
            anim.Play("HideUpgrades");
            SetTextVisible(ButtonText, false);
        }
    }

    public void PlayGame()
    {
        //if(AudioManager.instance) AudioManager.instance.PlaySFX(buttonPressSFX);
        //SceneManager.LoadScene("Game");
        Debug.Log("Start Game pressed");
    }

    public void QuitGame()
    {
        //if(AudioManager.instance) AudioManager.instance.PlaySFX(buttonPressSFX);
        //Application.Quit();
        Debug.Log("Quit Game pressed");
    }

}
