using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEggController : MainController
{
    public GameObject prefabToSpawn;
    public GameObject shellTop;
    public GameObject shellBottom;
    private bool hasCracked = false;

    private void OnCollisionEnter(Collision collision) {
        if(collision.relativeVelocity.magnitude > 30 && !hasCracked) {
            hasCracked = true;
            if (transform.childCount < 1) { return; }
            GameObject topShell = transform.GetChild(0).gameObject;
            GameObject newTop = Instantiate(shellTop, topShell.transform.position, topShell.transform.rotation);
            GameObject newBottom = Instantiate(shellBottom, transform.position, transform.rotation);
            GameObject prefab = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Destroy(topShell);
            Destroy(this.gameObject);
        }
    }
}
