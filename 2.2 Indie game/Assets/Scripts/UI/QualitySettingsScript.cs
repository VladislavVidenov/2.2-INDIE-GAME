using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QualitySettingsScript : SettingsScript {
    public bool expensiveChanges = true;

    private string[] qualityLevels;
    private int currentQualityLevel;

    [Tooltip("Add all quality toggles from the 'Lowest' to the 'Best' setting.")]
    public Toggle[] qualityToggles;

    void Awake() {
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