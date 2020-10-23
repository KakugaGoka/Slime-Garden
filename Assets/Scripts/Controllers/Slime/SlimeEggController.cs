using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEggController : MainController
{

    private Rigidbody m_RigidBody;
    public GameObject prefabToSpawn;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.relativeVelocity.magnitude > 30) {
            if (transform.childCount < 1) { return; }
            GameObject topShell = transform.GetChild(0).gameObject;
            InteractHold hold = topShell.AddComponent<InteractHold>();
            Rigidbody rigidBody = topShell.AddComponent<Rigidbody>();
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            InteractController interact = hold.GetComponent<InteractController>();
            InteractController thisInteract = GetComponent<InteractController>();
            InteractHold thisHold = GetComponent<InteractHold>();
            string message = "Pick Up Egg Shell (E)";
            interact.interactionMessage = message;
            thisInteract.interactionMessage = message;
            interact.messageColor = thisInteract.messageColor;
            hold.value = 20;
            thisHold.value = 20;
            topShell.transform.parent = null;
            name = "Egg Bottom";
            tag = "Toy";
            topShell.tag = "Toy";
            gameObject.AddComponent<ToyController>();
            topShell.AddComponent<ToyController>();
            GameObject prefab = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            prefab.name = "Baby " + prefab.name.Replace("(Clone)", "");
            Destroy(this);
        }
    }
}
