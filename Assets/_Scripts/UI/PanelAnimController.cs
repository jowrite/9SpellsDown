using UnityEngine;

public class PanelAnimController : MonoBehaviour
{
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenPanel()
    {
        animator.SetBool("isOpen", true);
    }

    public void ClosePanel()
    {
        animator.SetBool("isOpen", false);
    }
}
