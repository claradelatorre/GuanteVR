using UnityEngine;

public class StartButtonScript : MonoBehaviour
{
    public GameController gameController; 
    
    //LLAMA AL MÉTODO STARTGAME CUANDO SE HACE CLIC EN EL BOTÓN START
    public void OnStartButtonClick()
    {
        gameController.StartGame(); 
    }
}
