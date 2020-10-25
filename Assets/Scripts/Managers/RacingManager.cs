using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingManager : MonoBehaviour
{
    public List<SlimeController> slimes;

    private void Start(){
        foreach (var item in FindObjectsOfType<SlimeController>()) {
            slimes.Add(item);
        }
        StartWaiting();
        StartCoroutine(Wait(3));
    }

    public void GetSlime(int numberOfSlimes) {

    }

    public void StartRacing() {
        for (int i = 0; i < slimes.Count; i++) {
            slimes[i].activity = SlimeController.Activity.Racing;
        }
    }

    public void StartWaiting() {
        for (int i = 0; i < slimes.Count; i++) {
            slimes[i].activity = SlimeController.Activity.Waiting;
        }
    }

    IEnumerator Wait(int seconds) {
        yield return new WaitForSeconds(seconds);
        StartRacing();
    }
}