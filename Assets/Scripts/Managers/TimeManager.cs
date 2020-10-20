using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    [Tooltip("This is the Text item in your HUD that you would like to have the time printed out to.")]
    public Text clock;

    [Tooltip("The current amount of time spent in game, in seconds.")]
    public float currentTimeInGame = 12 * secondsInAnHour;

    [Tooltip("This controls how fast the game simulation is. 1 is realtime")]
    [Range(0f, 100f)]
    public float gameTimeScale = 1f;

    [Tooltip("This controls how fast the day night cylce is. 1 is realtime. gameTimeScale effects this as well.")]
    [Range(0f, 86400f)]
    public float cycleTimeScale = 1f;

    static int secondsInAMinute = 60;
    static int minutesInAnHour = 60;
    static int hoursInADay = 24;
    enum DaysOfTheWeek {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    
    public int currentSeconds { get { return (int)currentTimeInGame % secondsInAMinute; } }
    public int currentMinutes { get { return (int)currentTimeInGame / secondsInAMinute % minutesInAnHour; } }
    public int currentHours { get { return (int)currentTimeInGame / secondsInAnHour % hoursInADay; } }
    public int currentDays { get { return (int)currentTimeInGame / secondsInADay; } }

    private string clockString { get {
            string minitues = currentMinutes.ToString();
            if (currentMinutes < 10) {
                minitues = "0" + minitues;
            }
            string hours = currentHours.ToString();
            if (currentHours < 10) {
                hours = "0" + hours;
            }
            return hours + ":" + minitues; 
        } 
    }

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
    static int secondsInAnHour { get { return secondsInAMinute * minutesInAnHour; } }
    static int secondsInADay { get { return minutesInADay * secondsInAMinute; } }
    static int minutesInADay { get { return minutesInAnHour * hoursInADay; } }

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
        return (currentTimeInGame / secondsInADay) * 360 + 270; // convert to degrees, and then offset by 270 to make Vector3.down happen at noon.
    }

    public void SetTime(float timeInSeconds) {
        currentTimeInGame = timeInSeconds;
    }

    public void SetTime(Vector3 dayHourMinute) {
        currentTimeInGame = (dayHourMinute.x * secondsInADay) + (dayHourMinute.y * secondsInAnHour) + (dayHourMinute.z * secondsInAMinute);
    }
}
