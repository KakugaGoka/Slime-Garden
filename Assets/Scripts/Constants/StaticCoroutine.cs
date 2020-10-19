using UnityEngine;
using System.Collections;

public class StaticCoroutine : MonoBehaviour {
    static public StaticCoroutine instance; //the instance of our class that will do the work

    void Awake() { //called when an instance awakes in the game
        instance = this; //set our static reference to our newly initialized instance
    }

    IEnumerator PerformCoroutine() { //the coroutine that runs on our monobehaviour instance
        while (true) {
            yield return 0;
        }
    }

    virtual public void DoCoroutine() {
        instance.StartCoroutine("PerformCoroutine"); //this will launch the coroutine on our instance
    }
}