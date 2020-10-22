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

    public GameObject slimePrefab;
    public GameObject[] treePrefabs;
    public GameObject[] toyPrefabs;

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

        PlayerPrefs.SetInt("SlimeCount", GetTagList("Slime").Length);
        PlayerPrefs.SetInt("TreeCount", GetTagList("Tree").Length);
        PlayerPrefs.SetInt("ToyCount", GetTagList("Toy").Length);

        SaveObjectType("Slime");
        SaveObjectType("Tree");
        SaveObjectType("Toy");
    }

    private static GameObject[] GetTagList(string tag) {
        return GameObject.FindGameObjectsWithTag(tag);
    }

    private static void SaveObjectType(string tag) {
        if (!Directory.Exists(savePath + tag)) {
            Directory.CreateDirectory(savePath + tag);
        }

        foreach (var item in Directory.GetFiles(savePath + tag)) {
            File.Delete(item);
        }

        GameObject[] objs = GetTagList(tag);

        foreach (var obj in objs) {
            string objName = obj.name.Replace(' ', '_');
            MainController info = obj.GetComponent<MainController>();
            if (info) {
                info.GetTransformData();
                string entityJSON = JsonUtility.ToJson(info, true);
                string filePath = savePath + tag + "/" + objName + ".json";
                File.WriteAllText(filePath, entityJSON);
                Debug.Log("Saved :: " + objName + " to JSON");
            }
        }
    }

    private static void LoadGardenData() {
        LoadObjectType("Slime");
        LoadObjectType("Tree");
        LoadObjectType("Toy");
    }

    private static void LoadObjectType(string tag) {
        int count = PlayerPrefs.GetInt(tag + "Count", 0);
        if (count < 1) { return; }

        string[] info = Directory.GetFiles(savePath + tag);

        foreach (var item in GetTagList(tag)) {
            Destroy(item);
        }

        for (int i = 0; i < count; i++) {
            if (i < info.Length) {
                string fileName = Path.GetFileNameWithoutExtension(info[i]).Replace('_', ' ');
                GameObject obj;
                if (tag == "Tree") {
                    GameObject toSpawn = main.treePrefabs[0];
                    foreach (GameObject gameObj in main.treePrefabs) {
                        if (gameObj.name.Contains(fileName)) {
                            toSpawn = gameObj;
                        }
                    }
                    obj = Instantiate(toSpawn);
                } else if (tag == "Toy") {
                    GameObject toSpawn = main.toyPrefabs[0];
                    foreach (GameObject gameObj in main.toyPrefabs) {
                        if (gameObj.name.Contains(fileName)) {
                            toSpawn = gameObj;
                        }
                    }
                    obj = Instantiate(toSpawn);
                } else {
                    obj = Instantiate(main.slimePrefab);
                }
                MainController controller = obj.GetComponent<MainController>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(info[i]), controller);
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