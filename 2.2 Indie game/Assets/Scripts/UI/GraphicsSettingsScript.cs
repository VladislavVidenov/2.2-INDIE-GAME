using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GraphicsSettingsScript : SettingsScript {
    [Header("Brightness")]
    public Toggle customBrightness;
    public Slider brightnessSlider;
    public float middleGray = 0.1f;
    
    private Tonemapping toneMappingScript;


    [Header("Quality")]
    public bool expensiveChanges = true;
    private string[] qualityLevels;
    private int currentQualityLevel;

    [Tooltip("Add all quality toggles from the 'Lowest' to the 'Best' setting.")]
    public Toggle[] qualityToggles;

    

    void Awake() {
        //Brightness
        toneMappingScript = Camera.main.GetComponent<Tonemapping>();
        toneMappingScript.middleGrey = middleGray;
        
        //Quality
        qualityLevels = QualitySettings.names;                                      //Check available quality settings and store them in an array;
        currentQualityLevel = QualitySettings.GetQualityLevel();                    //Check the current quality setting and store it in a variable;
        qualityToggles[currentQualityLevel].isOn = true;
    }

    public void SetQualitySetting(int qualitySettingIndex, GameObject toggleGo) {
        QualitySettings.SetQualityLevel(qualitySettingIndex, expensiveChanges);
        currentQualityLevel = qualitySettingIndex;
    }

    //Overloading
    public void SetQualitySetting(string qualitySettingName) {
        for (int i = 0; i < qualityLevels.Length; i++) {
            if (qualityLevels[i] == qualitySettingName) {
                QualitySettings.SetQualityLevel(i, expensiveChanges);
                currentQualityLevel = i;
            }
        }
    }
}