using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour {
    public static SaveLoadManager main;
    public static bool isMenuOpen = true;
    private static PlayerCharacterController player;
    private static StaticCoroutine instance;

    void Start() {
        DontDestroyOnLoad(this);
        if (main == null) {
            main = this;
        } else {
            Destroy(gameObject);
            return;
        }
        instance = gameObject.AddComponent<StaticCoroutine>();
    }

    void Update() {

        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static void SaveAndQuit() {
        SaveGame();
        GameFlowManager.paused = false;
        QuitFromGame();
    }

    public static void SaveGame() {
        PlayerPrefs.SetInt("currentScene", SceneManager.GetActiveScene().buildIndex);
        SavePlayerData();
        //SaveGardenData();
        Debug.Log("Game Saved");
    }

    public static void LoadGame() {
        instance.StartCoroutine(LoadGameEnumerator("currentScene"));
    }

    private static void SavePlayerData() {
        PlayerPrefs.SetFloat("POSX", player.transform.position.x);
        PlayerPrefs.SetFloat("POSY", player.transform.position.y);
        PlayerPrefs.SetFloat("POSZ", player.transform.position.z);
        PlayerPrefs.SetFloat("ROTX", player.transform.rotation.x);
        PlayerPrefs.SetFloat("ROTY", player.transform.rotation.y);
        PlayerPrefs.SetFloat("ROTZ", player.transform.rotation.z);
        PlayerPrefs.SetInt("Wealth", player.wealth);
    }

    private static void LoadPlayerData() {
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("POSX"),
                                                PlayerPrefs.GetFloat("POSY"),
                                                PlayerPrefs.GetFloat("POSZ"));
        player.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("ROTX"),
                                                     PlayerPrefs.GetFloat("ROTY"),
                                                     PlayerPrefs.GetFloat("ROTZ"));
        player.wealth = PlayerPrefs.GetInt("Wealth");
    }

    private static void SaveGardenData() {
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

    private static void LoadGardenData() {

    }

    static IEnumerator<object> LoadGameEnumerator(string level) {
        int scene = PlayerPrefs.GetInt(level) == 0 ? 1 : PlayerPrefs.GetInt(level);
        var asyncLoadLevel = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoadLevel.isDone) {
            print("Loading the Scene");
            yield return null;
        }
        var wait = new WaitForFixedUpdate();
        yield return wait;
        isMenuOpen = false;
        player = GameFlowManager.main.player;
        LoadPlayerData();
        //LoadGardenData();
        Debug.Log("Game Loaded");
        yield return null;
    }

    public static void QuitFromGame() {
        SceneManager.LoadScene(0);
    }

    public static void QuitFromMenu() {
        Application.Quit();
    }
}
