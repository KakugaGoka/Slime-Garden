using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeController : MonoBehaviour {
    public float currentAge = 0;
    public float fullGrown = 300;
    public float deathAge = 300;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public int scaleSteps = 10;
    public bool destroyOnDieAge = false;
    public bool customAging = false;
    public float currentScale;
    private Vector3 startingScale;
    private float previousGrowth = 0;

    void Awake() {
        startingScale = gameObject.transform.localScale;
        currentScale = minScale;
        SetScale();
    }

    void Update() {
        if (!customAging) {
            Age();
            GetScale();
            SetScale();
        }
        if (destroyOnDieAge && currentAge >= deathAge) {
            Destroy(gameObject);
        }
    }

    void GetScale() {
        if (currentAge - previousGrowth >= fullGrown / scaleSteps) {
            previousGrowth = currentAge;
            float step = (maxScale - minScale) / scaleSteps;
            currentScale = Mathf.Clamp(currentScale + step, minScale, maxScale);
        }
    }

    public void SetScale() {
        gameObject.transform.localScale = new Vector3(currentScale * startingScale.x,
                                                      currentScale * startingScale.y,
                                                      currentScale * startingScale.z);
    }

    public void Age() => currentAge += Time.deltaTime * Random.Range(0.1f, 1);
}
