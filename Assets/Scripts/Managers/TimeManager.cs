using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

    private static int secondsInAMinute = 60;
    private static int minutesInAnHour = 60;
    private static int hoursInADay = 24;

    public UnityAction OnTheHour;
    public UnityAction OnTheDay;

    private enum DaysOfTheWeek {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public int currentSecond { get { return (int)currentTimeInGame % secondsInAMinute; } }
    public int currentMinute { get { return (int)currentTimeInGame / secondsInAMinute % minutesInAnHour; } }
    public int currentHour { get { return (int)currentTimeInGame / secondsInAnHour % hoursInADay; } }
    public int currentDay { get { return (int)currentTimeInGame / secondsInADay; } }

    private string clockString {
        get {
            string minitues = currentMinute.ToString();
            if (currentMinute < 10) {
                minitues = "0" + minitues;
            }
            string hours = currentHour.ToString();
            if (currentHour < 10) {
                hours = "0" + hours;
            }
            return hours + ":" + minitues;
        }
    }

    public string dayOfTheWeek {
        get {
            int weekLength = Enum.GetNames(typeof(DaysOfTheWeek)).Length;
            int day = currentDay % weekLength;
            try {
                return Enum.GetName(typeof(DaysOfTheWeek), day);
            } catch {
                return "ERROR";
            }
        }
    }

    public static TimeManager main;
    private static int secondsInAnHour { get { return secondsInAMinute * minutesInAnHour; } }
    private static int secondsInADay { get { return minutesInADay * secondsInAMinute; } }
    private static int minutesInADay { get { return minutesInAnHour * hoursInADay; } }

    private int oldDay = 0;
    private int oldHour = 0;

    private void Start() {
        if (main == null) {
            main = this;
        } else {
            Destroy(gameObject);
            return;
        }
        OnTheDay += NewDay;
        OnTheHour += NewHour;
    }

    private void Update() {
        Time.timeScale = GameFlowManager.paused ? 0 : gameTimeScale;
        currentTimeInGame += Time.deltaTime * cycleTimeScale;
        clock.text = dayOfTheWeek + " " + clockString;
        if (currentHour != oldHour) {
            oldHour = currentHour;
            OnTheHour.Invoke();
            if (currentDay != oldDay) {
                oldDay = currentDay;
                OnTheDay.Invoke();
            }
        }

    }

    private void NewHour() {}
    private void NewDay() { }


    public float TimeToDegrees()
    {
        return (currentTimeInGame / secondsInADay) * 360 + 270; // convert to degrees, and then offset by 270 to make Vector3.down happen at noon.
    }

    public void SetTime( float timeInSeconds )
    {
        currentTimeInGame = timeInSeconds;
    }

    public void SetTime( Vector3 dayHourMinute )
    {
        currentTimeInGame = (dayHourMinute.x * secondsInADay) + (dayHourMinute.y * secondsInAnHour) + (dayHourMinute.z * secondsInAMinute);
    }
}