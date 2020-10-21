using System;
using UnityEngine;

[Serializable]
public struct floatWithBounds {
    public float current;
    public float min;
    public float max;

    public floatWithBounds(float current, float min, float max) {
        this.current = current;
        this.min = min;
        this.max = max;
    }
}

public class SlimeShaderController : MonoBehaviour {
    [Serializable]
    public struct Faces {
        public Texture2D happy;
        public Texture2D angry;
        public Texture2D sad;
        public Texture2D hungry;
    }

    [Tooltip("")]
    public Shader slimeShader;

    [Tooltip("")]
    public Color slimeColor = Color.white;

    [Tooltip("")]
    public Faces faces;

    [Tooltip("")]
    public floatWithBounds amplitude = new floatWithBounds(0.1f, 0.05f, 0.5f);

    [Tooltip("")]
    public floatWithBounds frequency = new floatWithBounds(5f, 1f, 10f);

    [Tooltip("")]
    public floatWithBounds brightness = new floatWithBounds(0.2f, 0.05f, 0.5f);

    [Tooltip("")]
    public floatWithBounds spinRate = new floatWithBounds(1f, 0.1f, 1.5f);

    [Tooltip("")]
    public floatWithBounds cellType = new floatWithBounds(1f, 0.1f, 1.5f);

    [Tooltip("")]
    public floatWithBounds cellDensity = new floatWithBounds(1f, 0.1f, 1.5f);

    [Tooltip("")]
    public floatWithBounds speckleDensity = new floatWithBounds(5f, 1f, 10f);

    [Tooltip("")]
    public floatWithBounds speckleBrightness = new floatWithBounds(0f, 0f, 1f);

    private Material m_Material;
    private MeshRenderer m_Renderer;
    private Color[] m_Colors = new Color[] {
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.white,
        Color.gray,
        Color.black
    };

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        if (!m_Renderer) { Debug.LogError("Renderer for SlimeShaderController is null"); }

        if (!slimeShader) { Debug.LogError("Shader for SlimeShaderController is null"); }

        m_Material = new Material(slimeShader);
        if (!m_Material) { Debug.LogError("Material for SlimeShaderController is null"); }

        RegernerateSlime();    
    }

    private void Update() {
        if (Input.GetKeyDown("[")) {
            RegernerateSlime();
        }
    }

    private void RegernerateSlime() {
        ChangeFaceTexture(faces.happy);
        GetAll();
        SetAllInShader();
    }

    private void GetAll() {
        GetAmplitude();
        GetBrightness();
        GetCellDensity();
        GetCellType();
        GetColor();
        GetFrequency();
        GetSpeckleBrightness();
        GetSpeckleDensity();
        GetSpinRate();
    }

    public void SetAllInShader() {
        SetAmplitudeInShader();
        SetBrightnessInShader();
        SetCellDensityInShader();
        SetCellTypeInShader();
        SetColorInShader();
        SetFrequencyInShader();
        SetSpeckleBrightnessInShader();
        SetSpeckleDensityInShader();
        SetSpinRateInShader();

        m_Renderer.material = m_Material;
    }

    public void ChangeFaceTexture(Texture2D face) {
        if (!face) { Debug.LogError("Face passed in for SlimeShaderController is null"); }
        m_Material.SetTexture("Texture2D_9FA12E2E", face);
    }

    private void GetAmplitude() {
        amplitude.current = UnityEngine.Random.Range(amplitude.min, amplitude.max);
    }

    private void GetFrequency() {
        frequency.current = UnityEngine.Random.Range(frequency.min, frequency.max);
    }

    private void GetBrightness() {
        brightness.current = UnityEngine.Random.Range(brightness.min, brightness.max);
    }

    private void GetSpinRate() {
        spinRate.current = UnityEngine.Random.Range(spinRate.min, spinRate.max);
    }

    private void GetCellType() {
        cellType.current = UnityEngine.Random.Range(cellType.min, cellType.max);
    }

    private void GetCellDensity() {
        cellDensity.current = UnityEngine.Random.Range(cellDensity.min, cellDensity.max);
    }

    private void GetSpeckleDensity() {
        speckleDensity.current = UnityEngine.Random.Range(speckleDensity.min, speckleDensity.max);
    }

    private void GetSpeckleBrightness() {
        speckleBrightness.current = UnityEngine.Random.Range(speckleBrightness.min, speckleBrightness.max);
    }

    private void GetColor() {
        slimeColor = m_Colors[UnityEngine.Random.Range(0, m_Colors.Length)];
    }

    private void SetAmplitudeInShader() {
        m_Material.SetFloat("_Amplitude", amplitude.current); //Property Reference for Amplitude found in the shader.
    }

    private void SetFrequencyInShader() {
        m_Material.SetFloat("_Frequency", frequency.current); //Property Reference for Frequency found in the shader.
    }

    private void SetBrightnessInShader() {
        m_Material.SetFloat("_Brightness", brightness.current); //Property Reference for Brightness found in the shader.
    }

    private void SetSpinRateInShader() {
        m_Material.SetFloat("_SpinRate", spinRate.current); //Property Reference for Spin Rate found in the shader.
    }

    private void SetCellTypeInShader() {
        m_Material.SetFloat("_CellType", cellType.current); //Property Reference for Cell Type found in the shader.
    }

    private void SetCellDensityInShader() {
        m_Material.SetFloat("_CellDensity", cellDensity.current); //Property Reference for Cell Density found in the shader.
    }

    private void SetSpeckleDensityInShader() {
        m_Material.SetFloat("_SpeckleDensity", speckleDensity.current); //Property Reference for Speckle Density found in the shader.
    }

    private void SetSpeckleBrightnessInShader() {
        m_Material.SetFloat("_SpeckleBrightness", speckleBrightness.current); //Property Reference for Speckle Brightness found in the shader.
    }

    private void SetColorInShader() {
        m_Material.SetColor("_Color", slimeColor); //Property Reference for Color found in the shader.
    }

}
