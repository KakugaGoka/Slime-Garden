using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgeController))]
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

    private MeshFilter treeLeavesMesh;
    private Transform treeLeavesTransform;
    [HideInInspector]
    public bool canProduceFruit = false;
    [HideInInspector]
    public bool canDropFruit = false;
    [HideInInspector]
    public bool didDropFruit = false;
    private float timer = 0;
    private AgeController m_Age;

    void Start() {
        treeLeavesTransform = transform.Find("Leaves");
        treeLeavesMesh = treeLeavesTransform.GetComponent<MeshFilter>();
        treeLeavesTransform.localPosition = new Vector3(0, UnityEngine.Random.Range(1.5f, 2.5f), 0);
        m_Age = GetComponent<AgeController>();
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
                if (treeLeavesTransform.childCount < 1) {
                    didDropFruit = true;
                }
            }
            for (int i = 0; i < treeLeavesTransform.childCount; i++) {
                AgeController age = treeLeavesTransform.GetChild(i).GetComponent<AgeController>();
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
            fruitList.Add(treeLeavesMesh.mesh.GetRandomPointOnMesh());
        }
        fruitSpots = fruitList.ToArray();
    }

    void SpawnFruit() {
        for (int i = 0; i < fruitSpots.Length; i++) {
            var newFruit = Instantiate(fruit);
            newFruit.transform.parent = treeLeavesTransform;
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
        var fruit = treeLeavesTransform.GetChild(fruitToDrop);
        fruit.transform.parent = null;
        Rigidbody rigidbody = fruit.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null) {
            rigidbody.useGravity = true;
        }
    }
}
