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
    public PlayerCharacterController player { get; private set; }
    public Canvas HUD;
    public Canvas pauseMenu;

    public static string savePath;

    public static bool paused = false;


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

        player = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerCharacterController>();
        savePath = Application.persistentDataPath + "/Saves";
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
}