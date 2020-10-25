using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RacingManager : MonoBehaviour
{
    public List<SlimeController> slimes;
    public GameObject racingSlimePrefab;
    public Text numerator;
    public Text denominator;
    public Text countdown;
    public int numberOfOtherSlimes = 3;
    
    [HideInInspector]
    public GameObject startingLine;
    [HideInInspector]
    public GameObject finishLine;
    [HideInInspector]
    public Camera raceCamera;

    public RaceCameraController camControl {  get { return raceCamera.GetComponent<RaceCameraController>(); } }

    private PlayerCharacterController m_Player;
    private SlimeController m_Slime;
    private List<SlimeController> finishedSlimes;
    private bool raceHasStarted = false;
    private string currentTrack;
    private int currentPlace;

    private void Update() {
        // Recurring setup so that only one racing manager is needed.
        if (!startingLine) {
            startingLine = GameObject.Find("StartingLine");
        }
        if (!finishLine) {
            finishLine = GameObject.Find("FinishLine");
        }
        if (!raceCamera) {
            GameObject camera = GameObject.Find("RaceCamera");
            if (camera) {
                raceCamera = camera.GetComponent<Camera>();
                if (raceCamera) {
                    raceCamera.enabled = false;
                }
            }
        }
        if (!m_Player) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player) m_Player = player.GetComponent<PlayerCharacterController>();
        }

        if (raceHasStarted) {
            // Need to determine slime's place in the race.
            slimes = slimes.OrderByDescending(x => x.spoint.tFull).ToList();
            for (int i = 0; i < slimes.Count; i++) {
                if (slimes[i].activity == SlimeController.Activity.Waiting && !finishedSlimes.Contains(slimes[i])) {
                    finishedSlimes.Add(slimes[i]);
                }
                if (slimes[i] == m_Slime) {
                    currentPlace = i + 1;
                    break;
                }
            }

            FormatPosition();
            
            // What to do at the end of the race
            if (m_Slime.activity == SlimeController.Activity.Waiting) {
                for (int i = 0; i < finishedSlimes.Count; i++) {
                    if(finishedSlimes[i] == m_Slime) {
                        currentPlace = i + 1;
                        FormatPosition();
                        break;
                    }
                }
                StartCoroutine(WaitToEnd());
                raceHasStarted = false;
            }
        }
    }

    private void FormatPosition() {
        switch (currentPlace % 10) {
            case 1:
                numerator.text = String.Format("{0}st", currentPlace.ToString()); break;
            case 2:
                numerator.text = String.Format("{0}nd", currentPlace.ToString()); break;
            case 3:
                numerator.text = String.Format("{0}rd", currentPlace.ToString()); break;
            default:
                numerator.text = String.Format("{0}th", currentPlace.ToString()); break;
        }

        switch (currentPlace) {
            case 11:
                numerator.text = String.Format("{0}th", currentPlace.ToString()); break;
            case 12:
                numerator.text = String.Format("{0}th", currentPlace.ToString()); break;
            case 13:
                numerator.text = String.Format("{0}th", currentPlace.ToString()); break;
            default:
                break;
        }
    }

    public void LoadRaceTrack(string trackName) {
        currentTrack = trackName;
        StartCoroutine(LoadRaceTrackCoroutine(trackName));
    }

    private IEnumerator LoadRaceTrackCoroutine(string trackName) {
        var track = SceneManager.LoadSceneAsync(trackName, LoadSceneMode.Additive);
        while (!track.isDone) {
            print("Loading the Track");
            yield return null;
        }
        print("Finished loading the Track");
        if (m_Player.heldItem) {
            m_Slime = m_Player.heldItem.GetComponent<SlimeController>();
            if (m_Slime) StartRace(m_Slime, numberOfOtherSlimes);
            yield return null;
        }
    }

    public void StartRace(SlimeController mySlime, int numberOfOtherSlimes) {
        denominator.text = (numberOfOtherSlimes + 1).ToString();
        m_Player.enabled = false;
        raceCamera.enabled = true;
        camControl.thingToFollow = mySlime.gameObject;
        slimes = new List<SlimeController>();
        finishedSlimes = new List<SlimeController>();
        slimes.Add(mySlime);
        InteractHold hold = mySlime.GetComponent<InteractHold>();
        hold.onDrop.Invoke(m_Player);
        mySlime.transform.position = startingLine.transform.position + new Vector3(0, 0, 2);
        GetSlime(numberOfOtherSlimes);
        StartWaiting();
        StartCoroutine(WaitToStart());
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

    IEnumerator WaitToStart() {
        countdown.text = 3.ToString();
        yield return new WaitForSeconds(1);
        countdown.text = 2.ToString();
        yield return new WaitForSeconds(1);
        countdown.text = 1.ToString();
        yield return new WaitForSeconds(1);
        countdown.text = "GO!";
        StartRacing();
        countdown.text = "";
    }

    IEnumerator WaitToEnd() {
        countdown.text = String.Format("FINISHED IN {0}!!", numerator.text);
        yield return new WaitForSeconds(3);
        countdown.text = "";
        InteractMenu menu = GetComponent<InteractMenu>();
        if (menu) {
            menu.CloseSubMenu();
        }
        InteractController interact = m_Slime.GetComponent<InteractController>();
        interact.onInteract.Invoke(m_Player);
        raceCamera.enabled = false;
        m_Player.enabled = true;
        for (int i = 0; i < slimes.Count; i++) {
            if (slimes[i] != m_Slime) {
                Destroy(slimes[i]);
            }
        }
        slimes = new List<SlimeController>();
        startingLine = null;
        finishLine = null;
        raceCamera = null;
        SceneManager.UnloadSceneAsync(currentTrack);
        m_Slime.activity = SlimeController.Activity.Garden;
    }
}