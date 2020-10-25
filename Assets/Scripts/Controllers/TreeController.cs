using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeController : MainController {

    [Tooltip("To positions along the leaves mesh that fruit will grow.")]
    [SerializeField]
    public Vector3[] fruitSpots;

    [Tooltip("The type of fruit that will grow on this tree.")]
    [SerializeField]
    public GameObject fruit;

    [Tooltip("The time between each production of fruit.")]
    [SerializeField]
    public float timeBetweenProduction = 5f;

    [Tooltip("The time between each production of fruit.")]
    [SerializeField]
    public float timeBeforeFalls = 5f;

    [Tooltip("The time between each production of fruit.")]
    [SerializeField]
    public float timeInbetweenFalls = 5f;

    public MeshFilter m_Mesh;
    public AgeController m_Age;
    [HideInInspector]
    public bool canProduceFruit = false;
    [HideInInspector]
    public bool canDropFruit = false;
    [HideInInspector]
    public bool didDropFruit = false;
    private float timer = 0;

    void Start() {
        GameObject soil = transform.Find("Soil").gameObject;
        if (soil) {
            GameObject tree = soil.transform.Find("Tree").gameObject;
            if (tree) {
                m_Age = tree.transform.GetComponentInChildren<AgeController>();
                m_Mesh = tree.transform.Find("Leaves").GetComponent<MeshFilter>();
            }
        }
        m_Mesh.transform.localPosition = m_Mesh.transform.localPosition + new Vector3(0, UnityEngine.Random.Range(-1, 1), 0);
        FruitPlacement();
        GameFlowManager.AddTree(this);
    }

    void Update() {
        if (m_Age.currentAge >= m_Age.fullGrown && fruit != null) {
            if (didDropFruit) {
                if (canProduceFruit) {
                    SpawnFruit();
                } else {
                    timer += Time.deltaTime;
                    if (timer >= timeBetweenProduction) {
                        canProduceFruit = true;
                        timer = 0;
                    }
                }
            } else {
                if (m_Mesh.transform.childCount < 1) {
                    didDropFruit = true;
                }
            }
            for (int i = 0; i < m_Mesh.transform.childCount; i++) {
                AgeController age = transform.GetChild(i).GetComponent<AgeController>();
                if (age) {
                    if (age.currentAge >= age.fullGrown) {
                        DropFruit(i);
                    }
                }
            }
        }
    }

    void FruitPlacement() {
        int fruit = UnityEngine.Random.Range(2, 5);
        List<Vector3> fruitList = new List<Vector3>();
        for (int i = 0; i < fruit; i++) {
            fruitList.Add(m_Mesh.mesh.GetRandomPointOnMesh());
        }
        fruitSpots = fruitList.ToArray();
    }

    void SpawnFruit() {
        for (int i = 0; i < fruitSpots.Length; i++) {
            var newFruit = Instantiate(fruit);
            newFruit.transform.parent = m_Mesh.transform;
            newFruit.transform.localPosition = fruitSpots[i];
            Rigidbody rigidbody = newFruit.GetComponent<Rigidbody>();
            if (rigidbody != null) {
                rigidbody.useGravity = false;
                rigidbody.Sleep();
            }
        }
        canProduceFruit = false;
        didDropFruit = false;
        FruitPlacement();
    }

    void DropFruit(int fruitToDrop) {
        var fruit = m_Mesh.transform.GetChild(fruitToDrop);
        fruit.transform.parent = null;
        Rigidbody rigidbody = fruit.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null) {
            rigidbody.useGravity = true;
        }
    }
}
