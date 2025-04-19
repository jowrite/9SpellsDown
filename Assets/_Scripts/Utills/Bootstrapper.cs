using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : Singleton<Bootstrapper>  
{
    public InputManager InputManager { get; private set; }

    #region Bootstrap
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BootstrapGame() => CheckScene("Bootstrapper");

    public static void CheckScene(string sceneName)
    {
        //traverse the current loaded scenes and if bootstrapper scene exists - exit the function
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
                return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        InputManager = gameObject.AddComponent<InputManager>();

        base.Awake();
        //start our game manager instance here
        //start our audio manager here

        


        CheckScene("Game");
    }
}
