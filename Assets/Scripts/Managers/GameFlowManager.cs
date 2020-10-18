﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager main { get; private set; }
    public List<TreeController> trees { get; private set; }
    public List<ShopController> shops { get; private set; }
    public float timeOfDay { get; private set; }

    [Tooltip("This is the main Directional or Poiunt light that you are using to illuminate your level.")]
    public Light sun;
    [Tooltip("This is the main game object that holds your level assets. This can also just be a gameObject at Vector3.zero")]
    public GameObject world;
    [Tooltip("This is the Text item in your HUD that you would like to have the time printed out to.")]
    public Text clock;
    [Tooltip("This is the Text where you show the day of the week.")]
    public Text dayOfTheWeek;
    [Tooltip("This controls how fast the day night cylce is. a scale of 1 mean that an hour is about 15 seconds. 0.25 means an hour is about a minute.")]
    [Range(0f, 50f)]
    public float timeScale = 0.25f;

    public static bool paused = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (main == null) {
            main = this;
        } else {
            Destroy(this);
        }
        trees = new List<TreeController>();
        shops = new List<ShopController>();
        timeOfDay = 21600f; //6am
    }

    void Update() {
        Time.timeScale = paused ? 0 : 1;
        TrackTime();
    }

    private void TrackTime() {
        timeOfDay += Time.deltaTime * 240 * timeScale;
        TimeSpan time = TimeSpan.FromSeconds((double)timeOfDay);
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
        clock.text = time.ToString(@"hh\:mm");
        sun.transform.RotateAround(world.transform.position, Vector3.forward, timeScale * Time.deltaTime);
    }

    public static void AddTree(TreeController tree) {
        main.trees.Add(tree);
    }

    public static void RemoveTree(TreeController tree) {
        main.trees.Remove(tree);
    }

    public static void AddShop(ShopController shop) {
        main.shops.Add(shop);
    }

    public static void RemoveShop(ShopController shop) {
        main.shops.Remove(shop);
    }
}
