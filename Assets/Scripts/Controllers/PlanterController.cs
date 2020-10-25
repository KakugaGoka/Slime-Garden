using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanterController : MainController
{
    public string plantMessage = "Plant";

    public void Plant(PlayerCharacterController player) {
        FruitController fruit = player.heldItem.GetComponent<FruitController>();
        if (fruit) {
            GameObject Planter = Instantiate(fruit.tree, transform.position, transform.rotation);
            Destroy(player.heldItem.gameObject);
            Destroy(gameObject);
        }
    }
}
