using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager main;
    public static bool isMenuOpen = true;
    private static PlayerCharacterController player;
    private static StaticCoroutine instance;

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
        instance = gameObject.AddComponent<StaticCoroutine>();
    }

    private void Update()
    {
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static void SaveAndQuit()
    {
        SaveGame();
        GameFlowManager.paused = false;
        QuitFromGame();
    }

    public static void SaveGame()
    {
        PlayerPrefs.SetInt( "currentScene", SceneManager.GetActiveScene().buildIndex );
        SavePlayerData();
        //SaveGardenData();
        Debug.Log( "Game Saved" );
    }

    public static void LoadGame()
    {
        instance.StartCoroutine( LoadGameEnumerator( "currentScene" ) );
    }

    private static void SavePlayerData()
    {
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

    private static void LoadGardenData()
    {
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
        //LoadGardenData();
        var wait = new WaitForFixedUpdate();
        yield return wait;
        Debug.Log( "Game Loaded" );
        yield return null;
    }

    public static void QuitFromGame()
    {
        SceneManager.LoadScene( 0 );
    }

    public static void QuitFromMenu()
    {
        Application.Quit();
    }
}