using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingManager : MonoBehaviour
{
    public List<SlimeController> slimes;
    public GameObject startingLine;
    public GameObject finishLine;
    public GameObject racingSlimePrefab;

    private bool raceHasStarted = false;

    private void Update() {
        if (slimes.TrueForAll(x => x.activity == SlimeController.Activity.Waiting) && raceHasStarted) {
            Debug.Log("RACE IS OVER!!!!");
        }
        if (Input.GetKeyDown("m")) {
            StartRace(Instantiate(racingSlimePrefab).GetComponent<SlimeController>(), 3);
        }
    }

    public void StartRace(SlimeController mySlime, int numberOfOtherSlimes) {
        slimes = new List<SlimeController>();
        slimes.Add(mySlime);
        mySlime.hopping = 100;
        mySlime.rolling = 100;
        mySlime.floating = 100;
        AgeController age = mySlime.GetComponent<AgeController>();
        age.currentAge = age.fullGrown;
        mySlime.transform.position = startingLine.transform.position + new Vector3(0, 0, 2);
        GetSlime(numberOfOtherSlimes);
        StartWaiting();
        StartCoroutine(WaitToStart(3));
    }

    public void GetSlime(int numberOfOtherSlimes) {
        for (int i = 0; i < numberOfOtherSlimes; i++) {
            GameObject newSlime = Instantiate(racingSlimePrefab, startingLine.transform.position + new Vector3(0, 0, (i + 2) * 2), Quaternion.identity);
            SlimeController slime = newSlime.GetComponent<SlimeController>();
            slimes.Add(slime);
            slime.hopping = 100;
            slime.rolling = 100;
            slime.floating = 100;
            AgeController age = slime.GetComponent<AgeController>();
            age.currentAge = age.fullGrown;
        }
    }

    public void StartRacing() {
        for (int i = 0; i < slimes.Count; i++) {
            slimes[i].activity = SlimeController.Activity.Racing;
        }
        raceHasStarted = true;
    }

    public void StartWaiting() {
        for (int i = 0; i < slimes.Count; i++) {
            slimes[i].activity = SlimeController.Activity.Waiting;
        }
    }

    IEnumerator WaitToStart(int seconds) {
        yield return new WaitForSeconds(seconds);
        StartRacing();
    }
}