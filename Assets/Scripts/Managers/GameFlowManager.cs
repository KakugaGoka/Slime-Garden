using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager main { get; private set; }
    public List<TreeController> trees { get; private set; }
    public List<ShopController> shops { get; private set; }
    public float timeInGame { get; private set; }
    public PlayerCharacterController player { get; private set; }
    public Canvas HUD;
    public Canvas pauseMenu;

    public static string savePath;

    [Tooltip( "This is the main Directional or Poiunt light that you are using to illuminate your level." )]
    public Light sun;

    [Tooltip( "This is the main game object that holds your level assets. This can also just be a gameObject at Vector3.zero" )]
    public GameObject world;

    [Tooltip( "This is the Text item in your HUD that you would like to have the time printed out to." )]
    public Text clock;

    [Tooltip( "This is the Text where you show the day of the week." )]
    public Text dayOfTheWeek;

    [Tooltip( "This controls how fast the day night cylce is." )]
    [Range( 0f, 50f )]
    public float timeScale = 1f;

    public static bool paused = false;

    [Range( 0, 23 )]
    public int hourOfDay = 12;

    public int daysInGame = 0;

    private int oldHourOfDay;
    private int oldDaysInGame;

    private void Awake()
    {
        if (main == null) {
            main = this;
        }
        else {
            Destroy( gameObject );
            return;
        }
        trees = new List<TreeController>();
        shops = new List<ShopController>();
        SetTime();

        player = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerCharacterController>();
        savePath = Application.persistentDataPath + "/Saves";
    }

    private void Update()
    {
        CheckTime();
        Time.timeScale = paused ? 0 : 1;
        TrackTime();
    }

    private void CheckTime()
    {
        if (daysInGame < 0) {
            daysInGame = Mathf.Max( daysInGame, 0 );
        }
        if (oldHourOfDay != hourOfDay || oldDaysInGame != daysInGame) {
            SetTime();
        }
    }

    private void SetTime()
    {
        float oldTime = timeInGame;
        timeInGame = (daysInGame * 24 * 60 * 60) + hourOfDay * 60 * 60;
        oldDaysInGame = daysInGame;
        oldHourOfDay = hourOfDay;
    }

    private void TrackTime()
    {
        if (sun == null || world == null || clock == null || dayOfTheWeek == null) {
            return;
        }
        timeInGame += Time.deltaTime * 240 * timeScale;
        TimeSpan time = TimeSpan.FromSeconds( (double)timeInGame );
        int day = time.Days % 7;
        switch (day) {
            case 0:
                dayOfTheWeek.text = "Sunday"; break;
            case 1:
                dayOfTheWeek.text = "Monday"; break;
            case 2:
                dayOfTheWeek.text = "Tuesday"; break;
            case 3:
                dayOfTheWeek.text = "Wednesday"; break;
            case 4:
                dayOfTheWeek.text = "Thursday"; break;
            case 5:
                dayOfTheWeek.text = "Friday"; break;
            case 6:
                dayOfTheWeek.text = "Saturday"; break;
        }
        clock.text = time.ToString( @"hh\:mm" );
        sun.transform.rotation = Quaternion.Euler( (timeInGame / 24 / 60 / 60) * 360 + 270, 0, 0 );
    }

    public static void AddTree( TreeController tree )
    {
        main.trees.Add( tree );
    }

    public static void RemoveTree( TreeController tree )
    {
        main.trees.Remove( tree );
    }

    public static void AddShop( ShopController shop )
    {
        main.shops.Add( shop );
    }

    public static void RemoveShop( ShopController shop )
    {
        main.shops.Remove( shop );
    }

    public static void Pause()
    {
        paused = !paused;
        SaveLoadManager.isMenuOpen = paused;
        main.HUD.gameObject.SetActive( !paused );
        main.pauseMenu.gameObject.SetActive( paused );
    }

    public static void SetTime( float time )
    {
        main.timeInGame = time;
    }
}