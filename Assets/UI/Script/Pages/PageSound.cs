using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PageSound : BasePage
{
    const string volumePrefString = "Volume_Main";
    private static float mainVolume = 100f;

    // Volume
    private Slider volumeSlider;
    private VisualElement volumeSliderFillPart;
    private const float baseMainVolume = 100f;


    public float MainVolume
    {
        get => mainVolume;
        set
        {
            mainVolume = value;
            PlayerPrefs.SetFloat(volumePrefString, MainVolume);
            volumeSliderFillPart.style.width = Length.Percent(MainVolume);
            AudioListener.volume = MainVolume / 100f;
            volumeSlider.label = "Volume ( " + (int) MainVolume + "% )";
        }
    }

    public PageSound(UIDocument uIDocument, VisualElement pageRoot, int depth) : base(uIDocument, pageRoot, depth) { }

    protected override void FetchUiElements() {
        // Volume
        volumeSlider = pageRoot.Q<Slider>("sound");
        volumeSliderFillPart = volumeSlider.Q<VisualElement>("unity-tracker");

    }

    protected override void LoadSettings() {
        if (PlayerPrefs.HasKey(volumePrefString))
        MainVolume = PlayerPrefs.GetFloat(volumePrefString);
        volumeSlider.value = MainVolume;
    }

    protected override void SetPageBehavior() {        
        volumeSlider.RegisterValueChangedCallback( e => MainVolume = e.newValue );
    }

    public override void LoadBaseSettings() {
        PlayerPrefs.SetFloat(volumePrefString, baseMainVolume);
        volumeSlider.value = MainVolume;
    }
}