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
    public Button buyButton;
    public Button sellButton;

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
            Text itemText = itemTile.GetComponentInChildren<Text>();
            Text itemValue = itemText.transform.GetChild(0).GetComponent<Text>();
            RawImage itemImage = itemTile.GetComponentInChildren<RawImage>();
            RenderTexture texture = new RenderTexture(300, 300, 40);
            Camera camera = new GameObject().AddComponent<Camera>();
            GameObject item = Instantiate(itemsForSale[i].prefab, camera.gameObject.transform);

            itemText.text = itemsForSale[i].prefab.name;
            itemValue.text = itemsForSale[i].value.ToString() + "©";
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
            camera.transform.position = new Vector3(0, 500 + (50 * i), 0);
            camera.nearClipPlane = 0.01f;
            camera.targetTexture = texture;
            itemImage.texture = texture;
            item.tag = "Untagged";
            item.transform.localPosition = itemsForSale[i].position;
            item.transform.localRotation = Quaternion.Euler(itemsForSale[i].rotation);

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb) {
                rb.useGravity = false;
                rb.freezeRotation = true;
                rb.detectCollisions = false;
            }

            MeshRenderer mesh = item.GetComponent<MeshRenderer>();
            if (mesh) {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            AgeController age = item.GetComponent<AgeController>();
            if (age) {
                age.currentAge = age.fullGrown;
            }

            textures.Add(texture);
            itemTiles.Add(itemTile.GetComponent<Image>());
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
            buyButton.interactable = player.wealth >= itemsForSale[currentIndex].value && !player.isHolding;
            sellButton.interactable = player.isHolding;
        }
    }

    public void OnClick(Image image) {
        currentIndex = image.transform.GetSiblingIndex();
    }

    public void OnBuy() {
        if (player) {
            if (player.wealth >= itemsForSale[currentIndex].value) {
                GameObject newItem = Instantiate(itemsForSale[currentIndex].prefab);
                InteractController interact = newItem.GetComponent<InteractController>();
                if (interact) {
                    AgeController age = newItem.GetComponent<AgeController>();
                    if (age) {
                        age.currentAge = age.fullGrown;
                    }
                    interact.onInteract.Invoke(player);
                    player.wealth -= itemsForSale[currentIndex].value;
                } else {
                    Destroy(newItem);
                }
            }
        }
    }

    public void OnSell() {
        if (player) {
            player.wealth += player.heldItem.value;
            Destroy(player.heldItem.gameObject);
        }
    }
}
