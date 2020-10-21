using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeMenuController : MonoBehaviour
{
    [Serializable]
    public struct StatUI {
        public Text text;
        public Slider slider;

        public StatUI(Text text, Slider slider) {
            this.text = text;
            this.slider = slider;
        }

        public void Set(float current, float max) {
            text.text = ((int)current).ToString();
            slider.value = Mathf.Clamp(current / max, 0, 1);
        }
    }

    public Text slimeName;
    public StatUI hunger;
    public StatUI hopping;
    public StatUI rolling;
    public StatUI floating;
    public StatUI range;
}
