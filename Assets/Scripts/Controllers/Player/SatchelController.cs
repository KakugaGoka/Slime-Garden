using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatchelController : MonoBehaviour
{
    struct Pocket {
        public RawImage image;
        public Text text;

        public Pocket(RawImage image, Text text) {
            this.image = image;
            this.text = text;
        }
    }

    public int numberOfPockets = 3;
    public GameObject emptyObject;
    public GameObject playerSachelUI;
    public GameObject pocketPrefab;
    private PlayerCharacterController player;
    private List<Camera> slotCameras = new List<Camera>();
    private List<RenderTexture> slotTextures = new List<RenderTexture>();
    private List<GameObject> slotItems = new List<GameObject>();
    private List<Pocket> slotPockets = new List<Pocket>();

    private Vector3 startPosition = new Vector3(100, 500, 0);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        for (int i = 0; i < numberOfPockets; i++) {
            Camera camera = new GameObject().AddComponent<Camera>();
            RenderTexture texture = new RenderTexture(64, 64, 64);
            GameObject empty = Instantiate(emptyObject);
            GameObject pocketGO = Instantiate(pocketPrefab, playerSachelUI.transform);
            Pocket pocket = new Pocket(pocketGO.GetComponentInChildren<RawImage>(), pocketGO.GetComponentInChildren<Text>());

            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
            camera.transform.position = startPosition + new Vector3(0, 50 * i, 0);
            camera.nearClipPlane = 0.01f;
            camera.targetTexture = texture;

            pocket.image.texture = texture;
            pocket.text.text = (i+1).ToString();

            slotCameras.Add(camera);
            slotTextures.Add(texture);
            slotPockets.Add(pocket);
            slotItems.Add(empty);
        }

    }

    void Update()
    {
        for (int i = 1; i <= numberOfPockets; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                SwapItem(i);
            }
        }
        EnforceBeltSlots();
        SetActiveItem();
    }

    void EnforceBeltSlots() {
        if (player.heldObjectLocation.childCount < numberOfPockets + 1) {
            Instantiate(emptyObject, player.heldObjectLocation);
        }
    }

    void SetActiveItem() {
        for (int i = 1; i < player.heldObjectLocation.childCount; i++) {
            player.heldObjectLocation.GetChild(i).gameObject.SetActive(false);
        }
        player.heldObjectLocation.GetChild(0).gameObject.SetActive(true);
    }

    void SwapItem(int index) {
        if (player) {
            if (player.heldItem) {
                if (!player.heldItem.canBeStowed) {
                    return;
                }
            }
            Transform held = player.heldObjectLocation;
            List<GameObject> playerItems = new List<GameObject>();

            for (int i = 0; i < held.childCount; i++) {
                playerItems.Add(held.GetChild(i).gameObject);
            }
            playerItems[index].transform.SetSiblingIndex(0);
            playerItems[0].transform.SetSiblingIndex(index);
            player.heldItem = held.GetChild(0).GetComponent<InteractHold>();
            player.isHolding = held.GetChild(0).GetComponent<InteractHold>();

            GameObject item = Instantiate(held.GetChild(index).gameObject, slotCameras[index-1].transform);

            float distance = 0.5f;
            Collider collider = item.GetComponent<Collider>();
            if (collider) {
                Bounds bounds = ColliderBounds(item);
                distance = bounds.extents.x + bounds.extents.y + bounds.extents.z;
            }

            item.tag = "Untagged";
            item.transform.localPosition = new Vector3(0, 0, distance);
            item.transform.localRotation = Quaternion.identity;

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb) {
                rb.useGravity = false;
                rb.freezeRotation = true;
                rb.detectCollisions = false;
            }

            MeshRenderer rend = item.GetComponent<MeshRenderer>();
            if (rend) {
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Destroy(slotItems[index - 1]);
            slotItems[index - 1] = item;
        }
    }

    Bounds MeshBounds(GameObject target) {
        var bound = target.GetComponent<MeshFilter>().mesh.bounds;

        MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter filter in filters) {
            bound.Encapsulate(filter.mesh.bounds);
        }

        return bound;
    }

    Bounds ColliderBounds(GameObject target) {
        var bound = target.GetComponent<Collider>().bounds;

        Collider[] colliders = target.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders) {
            bound.Encapsulate(collider.bounds);
        }

        return bound;
    }
}
