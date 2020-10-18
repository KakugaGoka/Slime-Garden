using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager main { get; private set; }
    public List<TreeController> trees { get; private set; }
    public List<ShopController> shops { get; private set; }
    public float timeOfDay { get; private set; }
    public PlayerCharacterController player { get; private set; }

    public static string savePath;

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
            return;
        }
        trees = new List<TreeController>();
        shops = new List<ShopController>();
        timeOfDay = 21600f; //6am
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        savePath = Application.persistentDataPath + "/Saves";
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

    public void SaveGame() {
        PlayerPrefs.SetInt("currentScene", SceneManager.GetActiveScene().buildIndex);
        SavePlayerData();
        SaveGardenData();
        Debug.Log("Game Saved");
    }

    private void SavePlayerData() {

    }

    private void SaveGardenData() {
        //foreach (var file in Directory.GetFiles(savePath)) {
        //    File.Delete(file);
        //}

        //foreach (var obj in enemies) {
        //    string enemyName = obj.name.Replace(' ', '_');
        //    EnemyController enemy = obj.GetComponent<EnemyController>();
        //    if (enemy != null) {
        //        EnemyInfo info = new EnemyInfo(enemyName, obj.transform.position.x, obj.transform.position.y, enemy.currentHealth);
        //        string entityJSON = JsonUtility.ToJson(info);
        //        string filePath = savePath + enemyName + ".json";
        //        File.WriteAllText(filePath, entityJSON);
        //        Debug.Log("Saved :: " + enemyName + "to JSON");
        //    }
        //}
        //foreach (var obj in blocks) {
        //    string blockName = obj.name.Replace(' ', '_');
        //    HitBlockController block = obj.GetComponentInChildren<HitBlockController>();
        //    if (block != null) {
        //        BlockInfo info = new BlockInfo(blockName, block.activated);
        //        string entityJSON = JsonUtility.ToJson(info);
        //        string filePath = savePath + blockName + ".json";
        //        File.WriteAllText(filePath, entityJSON);
        //        Debug.Log("Saved :: " + blockName + "to JSON");
        //    }

        //}
    }

    public void LoadGame() {
        StartCoroutine("LoadGameEnumerator", "currentScene");
    }

    IEnumerator<object> LoadGameEnumerator(string level) {
        //var asyncLoadLevel = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt(level));
        //while (!asyncLoadLevel.isDone) {
        //    print("Loading the Scene");
        //    yield return null;
        //}
        //paused = false;
        //var wait = new WaitForFixedUpdate();
        //yield return wait;
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //player.loadPlayer = true;
        //LoadEntities();
        //Debug.Log("Game Loaded");
        yield return null;
    }
}
