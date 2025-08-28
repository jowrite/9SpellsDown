using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonPressSFX;

    [SerializeField] private Animator anim;

    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image soundToggleImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    //Panel visibility vars
    private bool isRulesPanelOpen = false;
    private bool isScoresPanelOpen = false;
    private bool isUpgradesPanelOpen = false;
    private bool isSettingsPanelOpen = false;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (AudioManager.am) AudioManager.am.PlayMusic(menuMusic);
    }

    //Taps will toggle the panel visibility
    public void ToggleRules()
    {
        isRulesPanelOpen = !isRulesPanelOpen;
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);

        if (isRulesPanelOpen)
        {
            anim.Play("ShowRulesPanel");
            SetTextVisible(buttonText, true);
        }
        else
        {
            anim.Play("HideRulesPanel");
            SetTextVisible(buttonText, false);
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
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);

        if (isScoresPanelOpen)
        {
            anim.Play("ShowScores");
            SetTextVisible(buttonText, true);
        }    
        else
        {
            anim.Play("HideScores");
            SetTextVisible(buttonText, false);
        }
    }
    public void ToggleUpgrades()
    {
        isUpgradesPanelOpen = !isUpgradesPanelOpen;
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);

        if (isUpgradesPanelOpen)
        {
            anim.Play("ShowUpgrades");
            SetTextVisible(buttonText, true);
        }
        else
        {
            anim.Play("HideUpgrades");
            SetTextVisible(buttonText, false);
        }
    }

    public void PlayGame()
    {
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);
        //SceneManager.LoadScene("Game");
        Debug.Log("Start Game pressed");
    }

    public void QuitGame()
    {
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);
        Application.Quit();
        Debug.Log("Quit Game pressed");
    }

    //Game Play scene audio settings

    public void Settings()
    {
        isSettingsPanelOpen = !isSettingsPanelOpen;
        if(AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);

        if (isSettingsPanelOpen)
        {
            anim.Play("ShowSettings");
            SetTextVisible(buttonText, true);
        }
        else
        {
            anim.Play("HideSettings");
            SetTextVisible(buttonText, false);
        }
        Debug.Log("Settings pressed");
    }

}
