﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    [Tooltip("This is the Text item in your HUD that you would like to have the time printed out to.")]
    public Text clock;

    [Tooltip("This controls how fast the game simulation is. 1 is realtime")]
    [Range(0f, 100f)]
    public float gameTimeScale = 1f;

    [Tooltip("This controls how fast the day night cylce is. 1 is realtime. gameTimeScale effects this as well.")]
    [Range(0f, 86400f)]
    public float cycleTimeScale = 1f;

    public enum DaysOfTheWeek {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public float currentTimeInGame;
    public Vector3 dayHourMinute = Vector3.zero; //Only used by the inspector

    public int currentSeconds { get { return (int)currentTimeInGame % secondsInAMinute; } }
    public int currentMinutes { get { return (int)currentTimeInGame / secondsInAMinute % minutesInAnHour; } }
    public int currentHours { get { return (int)currentTimeInGame / secondsInAnHour % hoursInADay; } }
    public int currentDays { get { return (int)currentTimeInGame / secondsInADay; } }

    private string clockString { get { return currentHours + ":" + currentMinutes; } }
    public string dayOfTheWeek {
        get {
            int weekLength = Enum.GetNames(typeof(DaysOfTheWeek)).Length;
            int day = currentDays % weekLength;
            try {
                return Enum.GetName(typeof(DaysOfTheWeek), day);
            } catch {
                return "ERROR";
            }
        }
    }

    public static TimeManager main;
    static int secondsInAMinute = 60;
    static int minutesInAnHour = 60;
    static int hoursInADay = 24;
    static int secondsInAnHour { get { return secondsInAMinute * minutesInAnHour; } }
    static int minutesInADay { get { return minutesInAnHour * hoursInADay; } }
    static int secondsInADay { get { return minutesInADay * secondsInAMinute; } }

    void Start() {
        if (main == null) {
            main = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Update() {
        Time.timeScale = GameFlowManager.paused ? 0 : gameTimeScale;
        currentTimeInGame += Time.deltaTime * cycleTimeScale;
        clock.text = dayOfTheWeek + " " + clockString;
    }

    public float TimeToDegrees() {
        return (currentTimeInGame / secondsInADay) * 360 + 270; // convert to degrees, and then offset by 180 to make Vector3.down happen at noon.
    }

    public void SetTime(float timeInSeconds) {
        currentTimeInGame = timeInSeconds;
    }

    public void SetTime(Vector3 dayHourMinute) {
        currentTimeInGame = (dayHourMinute.x * secondsInADay) + (dayHourMinute.y * secondsInAnHour) + (dayHourMinute.z * secondsInAMinute);
    }
}
