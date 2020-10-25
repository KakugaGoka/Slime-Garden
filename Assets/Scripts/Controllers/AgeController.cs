using System;
using UnityEngine;

[Serializable]
public class AgeController : MonoBehaviour {
    public float currentAge = 0;
    public float fullGrown = 300;
    public float deathAge = 300;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public bool destroyOnDieAge = false;
    public bool customAging = false;
    public bool showAgeByScaling = true;
    public float currentScale;
    public Vector3 startingScale;
    private float step {  get { return (maxScale - minScale); } }

    void Awake() {
        startingScale = transform.localScale;
        currentScale = minScale;
        if (showAgeByScaling) GetScale();
    }

    void Update() {
        if (!customAging)  Age();
        if (showAgeByScaling) GetScale();
        if (destroyOnDieAge && currentAge >= deathAge) {
            Destroy(gameObject);
        }
    }

    void GetScale() {
        float value = (currentAge / fullGrown) * step;
        currentScale = Mathf.Clamp(minScale + value, minScale, maxScale);
        transform.localScale = new Vector3(currentScale * startingScale.x,
                                           currentScale * startingScale.y,
                                           currentScale * startingScale.z);
    }

    public void Age() => currentAge += Time.deltaTime * UnityEngine.Random.Range(0.1f, 1);
}
