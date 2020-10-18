using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager main;
    private static PlayerCharacterController player;

    void Start() {
        DontDestroyOnLoad(this);
        if (main == null) {
            main = this;
        } else {
            Destroy(this);
            return;
        }
        player = GameFlowManager.main.player;
    }

    public void SaveGame() {
        PlayerPrefs.SetInt("currentScene", SceneManager.GetActiveScene().buildIndex);
        SavePlayerData();
        //SaveGardenData();
        Debug.Log("Game Saved");
    }

    public void LoadGame() {
        StartCoroutine("LoadGameEnumerator", "currentScene");
    }

    private void SavePlayerData() {
        PlayerPrefs.SetFloat("POSX", player.transform.position.x);
        PlayerPrefs.SetFloat("POSY", player.transform.position.y);
        PlayerPrefs.SetFloat("POSZ", player.transform.position.z);
        PlayerPrefs.SetFloat("ROTX", player.transform.rotation.x);
        PlayerPrefs.SetFloat("ROTY", player.transform.rotation.y);
        PlayerPrefs.SetFloat("ROTZ", player.transform.rotation.z);
        PlayerPrefs.SetInt("Wealth", player.wealth);
    }

    private void LoadPlayerData() {
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("POSX"),
                                                PlayerPrefs.GetFloat("POSY"),
                                                PlayerPrefs.GetFloat("POSZ"));
        player.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("ROTX"),
                                                     PlayerPrefs.GetFloat("ROTY"),
                                                     PlayerPrefs.GetFloat("ROTZ"));
        player.wealth = PlayerPrefs.GetInt("Wealth");
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

    private void LoadGardenData() {

    }

    IEnumerator<object> LoadGameEnumerator(string level) {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt(level));
        while (!asyncLoadLevel.isDone) {
            print("Loading the Scene");
            yield return null;
        }
        var wait = new WaitForFixedUpdate();
        yield return wait;
        LoadPlayerData();
        //LoadGardenData();
        Debug.Log("Game Loaded");
        yield return null;
    }
}
