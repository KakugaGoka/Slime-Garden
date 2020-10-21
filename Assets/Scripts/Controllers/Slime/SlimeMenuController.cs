using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeMenuController : MonoBehaviour
{
    [Serializable]
    public struct StatUI {
        public Text value;
        public Image fillBar;

        public StatUI(Text value, Image fillBar) {
            this.value = value;
            this.fillBar = fillBar;
        }

        public void SetValue(float val, float max) {
            value.text = ((int)val).ToString();
            Vector2 maxDelta = new Vector2(90, 20);
            fillBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp((val/max) * (maxDelta.x/max), 0, maxDelta.x), maxDelta.y);
        }
    }

    public Text slimeName;
    public StatUI hunger;
    public StatUI hopping;
    public StatUI rolling;
    public StatUI floating;
    public StatUI range;

    public bool isActive { get { return gameObject.activeInHierarchy; } }
}
