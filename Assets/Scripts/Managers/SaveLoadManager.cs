using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager main;
    public static bool isMenuOpen = true;
    private static PlayerCharacterController player;
    private static StaticCoroutine instance; 
    public static string savePath;

    public GameObject[] eggPrefab;
    public GameObject[] slimePrefab;
    public GameObject[] treePrefabs;
    public GameObject[] toyPrefabs;
    public GameObject[] foodPrefabs;

    private void Start()
    {
        DontDestroyOnLoad( this );
        if (main == null) {
            main = this;
        }
        else {
            Destroy( gameObject );
            return;
        }
        isMenuOpen = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
        instance = gameObject.AddComponent<StaticCoroutine>();
        savePath = Application.persistentDataPath + "/Saves/";
    }

    private void Update()
    {
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static void SaveAndQuit()
    {
        SaveGame();
        QuitFromGame();
    }

    public static void SaveGame()
    {
        PlayerPrefs.SetInt( "currentScene", SceneManager.GetActiveScene().buildIndex );
        SavePlayerData();
        SaveGardenData();
        Debug.Log( "Game Saved" );
    }

    public static void LoadGame()
    {
        instance.StartCoroutine( LoadGameEnumerator( "currentScene" ) );
    }

    private static void SavePlayerData()
    {
        player = GameFlowManager.main.player;

        PlayerPrefs.SetFloat( "POSX", player.transform.position.x );
        PlayerPrefs.SetFloat( "POSY", player.transform.position.y );
        PlayerPrefs.SetFloat( "POSZ", player.transform.position.z );
        PlayerPrefs.SetFloat( "ROTX", player.GetCameraAngle() );
        PlayerPrefs.SetFloat( "ROTY", player.transform.rotation.eulerAngles.y );
        PlayerPrefs.SetInt( "Wealth", player.wealth );
        PlayerPrefs.SetFloat( "CurrentTime", TimeManager.main.currentTimeInGame );

    }

    private static void LoadPlayerData()
    {
        player.transform.position = new Vector3( PlayerPrefs.GetFloat( "POSX" ),
                                                PlayerPrefs.GetFloat( "POSY" ),
                                                PlayerPrefs.GetFloat( "POSZ" ) );
        player.transform.rotation = Quaternion.Euler( 0, PlayerPrefs.GetFloat( "ROTY" ), 0 );
        player.SetCameraAngle( PlayerPrefs.GetFloat( "ROTX" ) );
        player.wealth = PlayerPrefs.GetInt( "Wealth" );
        TimeManager.main.SetTime( PlayerPrefs.GetFloat( "CurrentTime", 21600f ) );
    }

    private static void SaveGardenData()
    {
        if (!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }

        SaveObjectType("Slime");
        SaveObjectType("Tree");
        SaveObjectType("Toy");
        SaveObjectType("Egg");
        SaveObjectType("Food");
    }

    private static GameObject[] GetTagList(string tag) {
        return GameObject.FindGameObjectsWithTag(tag);
    }

    private static void SaveObjectType(string tag) {
        PlayerPrefs.SetInt(tag + "Count", GetTagList(tag).Length);

        if (!Directory.Exists(savePath + tag)) {
            Directory.CreateDirectory(savePath + tag);
        }

        foreach (var item in Directory.GetFiles(savePath + tag)) {
            File.Delete(item);
        }

        GameObject[] objs = GetTagList(tag);

        foreach (var obj in objs) {
            string objName = obj.name.Replace(' ', '_');
            string filePath = savePath + tag + "/" + objName + ".json";
            if (File.Exists(filePath)) {
                string append = "---";
                int index = 0;
                string finalPath = filePath + append + index;
                while (File.Exists(finalPath)) {
                    index++;
                    finalPath = filePath + append + index;
                }
                filePath = finalPath;
            }
            MainController info = obj.GetComponent<MainController>();
            if (info) {
                info.GetTransformData();
                string entityJSON = JsonUtility.ToJson(info, true);
                File.WriteAllText(filePath, entityJSON);
                Debug.Log("Saved :: " + objName + " to JSON");
            }
            AgeController age = obj.GetComponent<AgeController>();
            if (age) {
                string entityJSON = JsonUtility.ToJson(age, true);
                filePath = filePath.Replace(".json", "_Age.json");
                File.WriteAllText(filePath, entityJSON);
                Debug.Log("Saved :: " + objName + " to JSON");
            }
        }
    }

    private static void LoadGardenData() {
        LoadObjectType("Slime");
        LoadObjectType("Tree");
        LoadObjectType("Toy");
        LoadObjectType("Egg");
        LoadObjectType("Food");
    }

    private static void LoadObjectType(string tag) {
        int count = PlayerPrefs.GetInt(tag + "Count", 0);

        List<string> info = new List<string>();
        foreach (string path in Directory.GetFiles(savePath + tag)) {
            if (!path.Contains("_Age.json")) {
                info.Add(path);
            }
        }

        foreach (var item in GetTagList(tag)) {
            Destroy(item);
        }

        for (int i = 0; i < count; i++) {
            if (i < info.Count) {
                string fileName = Path.GetFileNameWithoutExtension(info[i]).Replace('_', ' ');
                GameObject obj;
                if (tag == "Tree") {
                    obj = InstantiateFromList(main.treePrefabs, fileName);
                } else if (tag == "Toy") {
                    obj = InstantiateFromList(main.toyPrefabs, fileName);
                } else if (tag == "Food") {
                    obj = InstantiateFromList(main.foodPrefabs, fileName);
                } else if (tag == "Egg") {
                    obj = InstantiateFromList(main.eggPrefab, fileName);
                } else { 
                    obj = InstantiateFromList(main.slimePrefab, fileName);
                }
                MainController controller = obj.GetComponent<MainController>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(info[i]), controller);
                if (File.Exists(info[i].Replace(".json", "_Age.json"))) {
                    AgeController age = obj.GetComponent<AgeController>();
                    if (age) {
                        JsonUtility.FromJsonOverwrite(File.ReadAllText(info[i].Replace(".json", "_Age.json")), age);
                    }
                }
                obj.transform.position = new Vector3(controller.PosX, controller.PosY, controller.PosZ);
                obj.transform.rotation = Quaternion.Euler(controller.RotX, controller.RotY + 1, controller.RotZ);
                obj.name = fileName;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb) {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.inertiaTensor = Vector3.one;
                    rb.Sleep();
                }
            }
        }
    }

    public static GameObject InstantiateFromList(GameObject[] prefabs, string fileName) {
        GameObject toSpawn = prefabs[0];
        foreach (GameObject gameObj in prefabs) {
            if (fileName.Contains(gameObj.name)) {
                toSpawn = gameObj;
                break;
            }
        }
        return Instantiate(toSpawn);
    }

    private static IEnumerator<object> LoadGameEnumerator( string level )
    {
        int scene = PlayerPrefs.GetInt( level ) == 0 ? 1 : PlayerPrefs.GetInt( level );
        var asyncLoadLevel = SceneManager.LoadSceneAsync( scene );
        while (!asyncLoadLevel.isDone) {
            print( "Loading the Scene" );
            yield return null;
        }
        isMenuOpen = false;
        player = GameFlowManager.main.player;
        LoadPlayerData();
        LoadGardenData();
        var wait = new WaitForFixedUpdate();
        yield return wait;
        Debug.Log( "Game Loaded" );
        yield return null;
    }

    public static void QuitFromGame() {
        GameFlowManager.paused = false;
        SceneManager.LoadScene( 0 );
    }

    public static void QuitFromMenu()
    {
        Application.Quit();
    }

    public static void NewGame() {
        SceneManager.LoadScene(1);
        isMenuOpen = false;
    }
}