using System;
using UnityEngine;

[Serializable]
public struct floatWithBounds {
    public float min;
    public float max;

    public floatWithBounds(float min, float max) {
        this.min = min;
        this.max = max;
    }
}

[Serializable]
public class SlimeShaderController : MonoBehaviour {
    private static floatWithBounds amplitude = new floatWithBounds(0.05f, 0.5f);
    private static floatWithBounds frequency = new floatWithBounds(1f, 10f);
    private static floatWithBounds brightness = new floatWithBounds(0.05f, 0.5f);
    private static floatWithBounds spinRate = new floatWithBounds(0.1f, 1.5f);
    private static floatWithBounds cellType = new floatWithBounds(0.1f, 1.5f);
    private static floatWithBounds cellDensity = new floatWithBounds(0.1f, 0.3f);
    private static floatWithBounds speckleDensity = new floatWithBounds(1f, 10f);
    private static floatWithBounds speckleBrightness = new floatWithBounds(0f, 1f);

    public static void ResetAllShaderValues(SlimeController slime) {
        if (!slime.lockAmplitude) slime.amplitude = GetValue(amplitude);
        if (!slime.lockBrightness) slime.brightness = GetValue(brightness);
        if (!slime.lockCellDensity) slime.cellDensity = GetValue(cellDensity);
        if (!slime.lockCellType) slime.cellType = GetValue(cellType);
        if (!slime.lockColor) slime.color = GetColor();
        if (!slime.lockFrequency) slime.frequency = GetValue(frequency);
        if (!slime.lockSpeckleBrightness) slime.speckleBrightness = GetValue(speckleBrightness);
        if (!slime.lockSpeckleDensity) slime.speckleDensity = GetValue(speckleDensity);
        if (!slime.lockSpinRate) slime.spinRate = GetValue(spinRate);
    }

    private static float GetValue(floatWithBounds minMax) {
        return UnityEngine.Random.Range(minMax.min, minMax.max);
    }

    private static Color GetColor() {
        return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
    }

}
