using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    [Tooltip("")]
    [SerializeField]
    public MeshFilter treeLeavesMesh;

    [Tooltip("")]
    [SerializeField]
    public Transform treeLeavesTransform;

    [Tooltip("The starting age of the tree.")]
    [SerializeField]
    public float startingAge = 30;

    [Tooltip("The age that the tree will begin to produce fruit.")]
    [SerializeField]
    public float maxAge = 300;

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

    private float currentScale = 0.1f;
    private float currentAge = 0;
    private bool canProduceFruit = false;
    private bool canDropFruit = false;
    private bool didDropFruit = false;
    private float timer = 0;

    void Start() {
        treeLeavesTransform.localPosition = new Vector3(0, Random.Range(1.5f, 2.5f), 0);
        FruitPlacement();
        GameFlowManager.AddTree(this);
    }

    void Update() {
        if (currentAge <= maxAge) { currentAge = Mathf.Clamp(AddTime(ref currentAge), 0, maxAge + 1); }
        if (currentScale != 1f) { GetScale(); }
        if (currentAge >= maxAge && fruit != null) {
            if (canProduceFruit) {
                if (didDropFruit) {
                    SpawnFruit();
                } else {
                    if (canDropFruit) {
                        if (timer >= timeInbetweenFalls) {
                            timer = 0;
                            if (treeLeavesTransform.childCount > 0) {
                                DropFruit();
                            } else {
                                didDropFruit = true;
                            }
                        } else {
                            AddTime(ref timer);
                        }
                    } else {
                        if (timer >= timeBeforeFalls) {
                            timer = 0;
                            canDropFruit = true;
                        } else {
                            AddTime(ref timer);
                        }
                    }
                }
            } else {
                AddTime(ref timer);
                if (timer >= timeBetweenProduction) {
                    canProduceFruit = true;
                    timer = 0;
                }
            }
        }
    }

    void FruitPlacement() {
        int fruit = Random.Range(2, 5);
        List<Vector3> fruitList = new List<Vector3>();
        for (int i = 0; i < fruit; i++) {
            fruitList.Add(treeLeavesMesh.mesh.GetRandomPointOnMesh());
        }
        fruitSpots = fruitList.ToArray();
        currentAge = startingAge;
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

    void DropFruit() {
        int fruitToDrop = Random.Range(0, treeLeavesTransform.childCount - 1);
        var fruit = treeLeavesTransform.GetChild(fruitToDrop);
        fruit.transform.parent = null;
        Rigidbody rigidbody = fruit.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null) {
            rigidbody.useGravity = true;
        }
    }

    void GetScale() {
        float value = (currentAge / maxAge) * 10;
        value = Mathf.Ceil(value) / 10;
        currentScale = Mathf.Clamp(value, 0.1f, 1);
        gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }

    float AddTime(ref float value) => value += Time.deltaTime * Random.Range(0.1f, 1);

}
