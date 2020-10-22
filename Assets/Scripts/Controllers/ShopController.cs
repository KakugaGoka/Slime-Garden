using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [Serializable]
    public struct ShopItem {
        public GameObject prefab;
        public int value;
        public Vector3 position;
        public Vector3 rotation;
        public string description;
    }

    public ShopItem[] itemsForSale;
    public GameObject itemTilePrefab;
    public GameObject forSaleList;
    public RawImage viewingImage;
    public Text viewingDescription;
    public Text playerWealth;

    List<RenderTexture> textures = new List<RenderTexture>();
    List<Image> itemTiles = new List<Image>();
    PlayerCharacterController player;

    Color unselected = new Color(1, 0.994f, 0.8573f);
    Color selected = Color.white;

    static int currentIndex = 0;
    static int oldIndex = 1;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        for (int i = 0; i < itemsForSale.Length; i++) {
            GameObject itemTile = Instantiate(itemTilePrefab, forSaleList.transform);
            itemTiles.Add(itemTile.GetComponent<Image>());
            Text itemText = itemTile.GetComponentInChildren<Text>();
            Text itemValue = itemText.transform.GetChild(0).GetComponent<Text>();
            RawImage itemImage = itemTile.GetComponentInChildren<RawImage>();
            itemText.text = itemsForSale[i].prefab.name;
            itemValue.text = itemsForSale[i].value.ToString() + "©";
            RenderTexture texture = new RenderTexture(300, 300, 40);
            Camera camera = new GameObject().AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
            camera.transform.position = new Vector3(0, 500 + (50 * i), 0);
            camera.nearClipPlane = 0.01f;
            camera.targetTexture = texture;
            itemImage.texture = texture;
            textures.Add(texture);
            GameObject item = Instantiate(itemsForSale[i].prefab, camera.gameObject.transform);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb) {
                rb.useGravity = false;
                rb.freezeRotation = true;
                rb.detectCollisions = false;
            }
            item.transform.localPosition = itemsForSale[i].position;
            item.transform.localRotation = Quaternion.Euler(itemsForSale[i].rotation);
            MeshRenderer mesh = item.GetComponent<MeshRenderer>();
            if (mesh) {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }

    private void Update() {
        if (currentIndex != oldIndex) {
            if (currentIndex < itemsForSale.Length) {
                viewingDescription.text = itemsForSale[currentIndex].description;
                viewingImage.texture = textures.ToArray()[currentIndex];
                for (int i = 0; i < itemTiles.Count; i++) {
                    if (i == currentIndex) {
                        itemTiles[i].color = selected;
                    } else {
                        itemTiles[i].color = unselected;
                    }
                }
            }
            oldIndex = currentIndex;
        }
        if (player) {
            playerWealth.text = player.wealth.ToString() + "©";
        }
    }

    public void OnClick(Image image) {
        currentIndex = image.transform.GetSiblingIndex();
    }

    public void OnBuy() {
        if (player) {
            if (player.isHolding) {
                return;
            } else {
                if (player.wealth >= itemsForSale[currentIndex].value) {
                    GameObject newItem = Instantiate(itemsForSale[currentIndex].prefab);
                    InteractController interact = newItem.GetComponent<InteractController>();
                    if (interact) {
                        interact.onInteract.Invoke(player);
                        player.wealth -= itemsForSale[currentIndex].value;
                    } else {
                        Destroy(newItem);
                    }
                }
            }
        }

    }
}
