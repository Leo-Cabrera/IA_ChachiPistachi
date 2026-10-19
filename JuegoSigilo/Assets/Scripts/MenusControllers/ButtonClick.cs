using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    private Animator animator;
    private bool isSelected = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

  
    public void OnClick()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        isSelected = !isSelected;
        animator.SetBool("IsSelected", isSelected);
    }
}
