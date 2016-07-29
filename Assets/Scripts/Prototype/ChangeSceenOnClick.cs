using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChangeSceenOnClick : MonoBehaviour
{
    public int scene;

	void Awake () 
    {
	    GetComponent<Button>().onClick.AddListener(OnClick);
	}

    void OnClick()
    { 
        SceneSwitcher.ChangeScene(scene);
    }
}
