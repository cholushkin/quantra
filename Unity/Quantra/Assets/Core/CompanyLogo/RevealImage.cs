using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class RevealRender : MonoBehaviour
{
    public float StartDelay;
    public float Duration;
    public Image Image;
    public Ease Ease;
    
    void Start()
    {
        Reveal();
    }

    void Reveal()
    {
        Image.DOFade(1f, Duration)
            .SetEase(Ease)
            .SetDelay(StartDelay);
    }
}
