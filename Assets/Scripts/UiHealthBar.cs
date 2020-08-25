using UnityEngine;
using UnityEngine.UI;

/**
 * Controller for game's UI - you can see Ruby's health there
 */
public class UiHealthBar : MonoBehaviour
{
    public static UiHealthBar Instance { get; private set; }
    
    public Image mask;
    private float _originalSize;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _originalSize * value);
    }
}
