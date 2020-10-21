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
    }

    public ShopItem[] itemsForSale;
    public GameObject itemTilePrefab;
    public GameObject forSaleList;

    void Start()
    {
        for (int i = 0; i < itemsForSale.Length; i++) {
            GameObject itemTile = Instantiate(itemTilePrefab, forSaleList.transform);
            Text itemText = itemTile.GetComponentInChildren<Text>();
            RawImage itemImage = itemTile.GetComponentInChildren<RawImage>();
            itemText.text = itemsForSale[i].prefab.name;
            RenderTexture texture = new RenderTexture(40, 40, 40);
            Camera camera = new GameObject().AddComponent<Camera>();
            camera.transform.position = new Vector3(0, 500 + (50 * i), 0);
            camera.nearClipPlane = 0.01f;
            camera.targetTexture = texture;
            itemImage.texture = texture;
            GameObject item = Instantiate(itemsForSale[i].prefab, camera.gameObject.transform);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb) { 
                rb.useGravity = false;
                rb.freezeRotation = true;
                rb.detectCollisions = false;
            }
            item.transform.localPosition = new Vector3(0, 0, 0.1f);
            item.transform.localRotation = Quaternion.Euler(-20, 0, 0);
            MeshRenderer mesh = item.GetComponent<MeshRenderer>();
            if (mesh) {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }


}
