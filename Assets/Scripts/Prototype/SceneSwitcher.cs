using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    int currentScene;

    const int hubSceneIndex = 0, uiSpeed = 5, barWidth = 480, barHeight = 18;

    readonly Vector2 barPos = new Vector2(0,0);

    float returnProgress, returnUIPos;

    readonly Color barColour = new Color(249,248,243);

    Texture2D barTex, lastGraphic;

    public Texture2D returnGraphic, exitGraphic;

    //Messy singleton.
    static SceneSwitcher singleton;

    bool returningToHub { get { return Input.GetKey(KeyCode.Escape) && currentScene != 0; } }
    bool ableToExit { get { return Input.GetKey(KeyCode.Escape) &&  currentScene == 0; } }

    void Awake()
    {
        //As I said - messy.
        if (singleton == null)
            singleton = this;

        if (singleton != this)
        {
            gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this);
    }

    public static void ChangeScene(int scene)
    {
        if (singleton != null)
            singleton.ChangeAndLoadScene(scene);
    }

    void ChangeAndLoadScene(int scene)
    {
        if (scene == currentScene)
            return;
       currentScene = scene;
       Application.LoadLevel(currentScene);
    }
    public void ExitingGame()
    {
        if (returnProgress >= 1)
        {
            Application.Quit();
            return;
        }
        returnProgress += Time.deltaTime * 0.5f;

        returnUIPos += Time.deltaTime * uiSpeed;
        returnUIPos = returnUIPos <= 1 ? returnUIPos : 1;
    }

    public void ReturnToHub()
    {
        if (returnProgress >= 1)
        {
            returnProgress = 0;
            returnUIPos = 0;
            ChangeScene(hubSceneIndex);
            Cursor.visible = true;
            return;
        }
        returnProgress += Time.deltaTime * 0.5f;
        
        returnUIPos += Time.deltaTime * uiSpeed;
        returnUIPos = returnUIPos <= 1 ? returnUIPos : 1;
    }

    public void ExitingReturnToHub()
    {
        returnProgress -= Time.deltaTime * 2.5f;
        returnProgress = returnProgress > 0 ? returnProgress : 0;
       
        returnUIPos -= Time.deltaTime * uiSpeed;
        returnUIPos = returnUIPos > 0 ? returnUIPos : 0;
    }

    void Update()
    {
        int uiTarget = returningToHub ? 1 : 0;
        if (returningToHub)
        {
            ReturnToHub();
        }
        else if(ableToExit)
        {
            ExitingGame();
        }
        else
        {
            ExitingReturnToHub();
        }
    }

    void OnGUI()
    {
        if (returnGraphic == null)
            return;
        if (barTex == null)
        {
            barTex = new Texture2D(1, 1);
            barTex.SetPixel(1, 1, barColour);
            barTex.Apply();
        }
        int xpos = (int)(-returnGraphic.width * (1f-returnUIPos)),
            ypos = Screen.height - returnGraphic.height;

        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(xpos,ypos,0),Quaternion.identity,Vector3.one);
        GUI.matrix = matrix;

        Texture2D bgGraphic = ableToExit ? exitGraphic : returningToHub ? returnGraphic : lastGraphic;

        //Draw graphic.
        Rect graphicRect = new Rect(0,0,returnGraphic.width,returnGraphic.height);
        GUI.DrawTexture(graphicRect, bgGraphic);

        //Draw bar.
        graphicRect = new Rect(15, 97, barWidth * returnProgress, barHeight);
        GUI.DrawTexture(graphicRect, barTex);

        GUI.matrix = Matrix4x4.identity;
        lastGraphic = bgGraphic;
    }
}
