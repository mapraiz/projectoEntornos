using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public GameObject startGame;
    public GameObject Quit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void exitGame(){
        Application.Quit();
    }
    public void gameStart(){
        SceneManager.LoadScene("Main");
    }
}
