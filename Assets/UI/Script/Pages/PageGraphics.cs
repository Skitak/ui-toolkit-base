using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PageGraphics : BasePage
{
    const string resolutionPrefString = "resolution";
    const string screenModePrefString = "screen-mode";


    private DropdownField resolutionDropdown;
    private Button screenModeButton;
    private FullScreenMode fullScreenMode = FullScreenMode.Windowed;

    public PageGraphics(UIDocument uIDocument, VisualElement pageRoot, int depth) : base(uIDocument, pageRoot, depth) { }

    protected override void FetchUiElements() {
        // Resolution
        resolutionDropdown = pageRoot.Q<DropdownField>("resolution");

        // Full Screen
        screenModeButton = pageRoot.Q<Button>("screen-mode");
    }

    protected override void LoadSettings() {
        // Resolution
        if (PlayerPrefs.HasKey(resolutionPrefString)) {
            string resolutionValue = PlayerPrefs.GetString(resolutionPrefString);
            ChangeResolution(resolutionValue);
        }

        // Screen mode
        if (PlayerPrefs.HasKey(screenModePrefString)) {
            string screenModeValue = PlayerPrefs.GetString(screenModePrefString);
            if (!fullScreenMode.ToString().Equals(screenModeValue))
                ToggleScreenMode();
        }

    }

    protected override void SetPageBehavior() {        
        // Resolution
        resolutionDropdown.choices = GetResolutionList();
        resolutionDropdown.value = Screen.width + "x" + Screen.height;
        resolutionDropdown.RegisterValueChangedCallback((e) => ChangeResolution(e.newValue));

        // Screen mode
        screenModeButton.text = fullScreenMode.ToString();
        screenModeButton.clickable.clicked += ToggleScreenMode;
    }

    private List<string> GetResolutionList() {
        List<string> resList = new List<string>(){};
        foreach (Resolution res in Screen.resolutions)
            resList.Add(res.width + "x" + res.height);
        return resList;
    }

    private void ChangeResolution(string newResolution) {
        int width = int.Parse(newResolution.Split('x')[0]);
        int height = int.Parse(newResolution.Split('x')[1]);
        Screen.SetResolution(width, height, fullScreenMode, 0);
        PlayerPrefs.SetString(resolutionPrefString, newResolution);
    }

    private void ToggleScreenMode() {
        if (fullScreenMode == FullScreenMode.FullScreenWindow) {
            screenModeButton.text = FullScreenMode.Windowed.ToString();
            fullScreenMode = FullScreenMode.Windowed;
        }
        else {
            screenModeButton.text = FullScreenMode.FullScreenWindow.ToString();
            fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        Screen.fullScreenMode = fullScreenMode;
        PlayerPrefs.SetString(screenModePrefString, fullScreenMode.ToString());
    }
}