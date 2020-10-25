using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEggController : MainController
{
    public GameObject prefabToSpawn;
    public GameObject shellTop;
    public GameObject shellBottom;
    public Color color;
    public float amplitude;
    public float frequency;
    public float brightness;
    public float spinRate;
    public float cellType;
    public float cellDensity;
    public float speckleDensity;
    public float speckleBrightness;
    public bool isInNest = false;

    private bool hasCracked = false;
    private InteractHold m_Hold;
    private AgeController m_Age;

    private void Start() {
        m_Hold = GetComponent<InteractHold>();
        m_Age = GetComponent<AgeController>();
    }

    private void Update() {
        if (m_Hold.isHeld || isInNest) {
            m_Age.Age();
        }
        if (m_Age.currentAge >= m_Age.fullGrown && !m_Age.customAging) {
            HatchEgg();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.relativeVelocity.magnitude > 30 && !hasCracked) {
            frequency = 10f;
            cellDensity = 0.5f;
            HatchEgg();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Nest") {
            isInNest = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Nest") {
            isInNest = false;
        }
    }

    private void HatchEgg() {
        hasCracked = true;
        if (transform.childCount < 1) { return; }
        GameObject topShell = transform.GetChild(0).gameObject;
        GameObject newTop = Instantiate(shellTop, topShell.transform.position, topShell.transform.rotation);
        GameObject newBottom = Instantiate(shellBottom, transform.position, transform.rotation);
        GameObject prefab = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        SlimeController slime = prefab.GetComponent<SlimeController>();
        if (slime) {
            SetSlimeValues(slime);
        }
        Destroy(topShell);
        Destroy(this.gameObject);
    }

    private void SetSlimeValues(SlimeController slime) {
        if (color != Color.clear) {
            slime.color = color;
            slime.lockColor = true;
        }
        if (amplitude != 0) {
            slime.amplitude = amplitude;
            slime.lockAmplitude = true;
        }
        if (frequency != 0) {
            slime.frequency = frequency;
            slime.lockFrequency = true;
        }
        if (brightness != 0) {
            slime.brightness = brightness;
            slime.lockBrightness = true;
        }
        if (spinRate != 0) {
            slime.spinRate = spinRate;
            slime.lockSpinRate = true;
        }
        if (cellType != 0) {
            slime.cellType = cellType;
            slime.lockCellType = true;
        }
        if (cellDensity != 0) {
            slime.cellDensity = cellDensity;
            slime.lockCellDensity = true;
        }
        if (speckleDensity != 0) {
            slime.speckleDensity = speckleDensity;
            slime.lockSpeckleDensity = true;
        }
        if (speckleBrightness != 0) {
            slime.speckleBrightness = speckleBrightness;
            slime.lockSpeckleBrightness = true;
        }
    }
}
