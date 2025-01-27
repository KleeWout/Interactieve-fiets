using UnityEngine;

public class hud_startup : MonoBehaviour
{

    public GameObject healthcontainer;
    void OnEnable(){
        if(GameSelect.gameMode == Models.GameModeModel.GameMode.SinglePlayer){
            healthcontainer.SetActive(false);
        }
        else{
            healthcontainer.SetActive(true);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
