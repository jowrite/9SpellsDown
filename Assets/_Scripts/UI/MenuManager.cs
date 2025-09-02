using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager mm;

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
    private bool isWildMagicPromptOpen = false;

    private void Awake()
    {
        if (mm == null) mm = this;
        else Destroy(gameObject);
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (AudioManager.am) AudioManager.am.PlayMusic(menuMusic);
        UpdateSoundToggle();
    }
    
    //Taps will toggle the panel visibility
    public void ToggleRules()
    {
        isRulesPanelOpen = !isRulesPanelOpen;
        PlayButtonSound();

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

    public void ToggleSound()
    {
        if (AudioManager.am)
        {
            AudioManager.am.ToggleMute();
            UpdateSoundToggle();
            PlayButtonSound();
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
        PlayButtonSound();

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
        PlayButtonSound();

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
        PlayButtonSound();        
        //SceneManager.LoadScene("Game");
        Debug.Log("Start Game pressed");
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
        Debug.Log("Quit Game pressed");
    }

    //Game Play scene audio settings

    public void Settings()
    {
        isSettingsPanelOpen = !isSettingsPanelOpen;
        PlayButtonSound();

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

    private void UpdateSoundToggle()
    {
        if (soundToggleImage && AudioManager.am)
        {
            soundToggleImage.sprite = AudioManager.am.IsMuted ? soundOffSprite : soundOnSprite;
        }
    }

    public void PromptWildMagic()
    {
        isWildMagicPromptOpen = !isWildMagicPromptOpen;
        PlayButtonSound();

        if (isWildMagicPromptOpen)
        {
            anim.Play("ShowWildMagicPrompt");
            SetTextVisible(buttonText, true);
        }
        else
        {
            anim.Play("HideWildMagicPrompt");
            SetTextVisible(buttonText, false);
        }
        Debug.Log("Wild Magic Prompt shown");
    }

    private void PlayButtonSound()
    {
        if (AudioManager.am) AudioManager.am.PlaySFX(buttonPressSFX);
    }

}
