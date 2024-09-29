using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletButtonToggleGroup : MonoBehaviour
{
    [SerializeField] private List<TabletButtonEventHandler> tabletButtons;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color disabledColor;

    private TabletButtonEventHandler currentButton;

    void Start()
    {
        foreach (var item in tabletButtons)
        {
            item.LinkToToggleGroup(this);
            item.ResetButton();
            item.SetDisabled();
        }
    }

    public (Color originalColor, Color selectedColor, Color disabledColor) GetColors()
    {
        return (originalColor, selectedColor, disabledColor);
    }

    public void SetCurrentButton(TabletButtonEventHandler btn)
    {
        if (currentButton != null && currentButton == btn) return;
        else
        {
            if (currentButton != null) currentButton.SetToggleOff();
            currentButton = btn;
            currentButton.SetToggleOn();
        }
    }

    public void ResetAllButtons()
    {
        foreach (var item in tabletButtons)
        {
            item.ResetButton();
        }
    }

    public void DisableAllButtons()
    {
        currentButton = null;
        foreach (var item in tabletButtons)
        {
            item.SetDisabled();
        }
    }

    public void EnableAllButtons()
    {
        foreach (var item in tabletButtons)
        {
            item.SetEnabled();
        }
    }

    public List<TabletButtonEventHandler> GetAllButtons()
    {
        return tabletButtons;
    }
}
