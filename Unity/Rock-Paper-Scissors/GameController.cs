using System.Collections;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TMP_Text roundText; 
    public TMP_Text countDownText;
    public TMP_Text detectingGestureText; 
    public TMP_Text rivalChoiceText;
    public GameObject startMenu; 
    public GameObject gamePanel; 
    public GameObject[] gestures; 
    private int currentRound = 0;
    private int playerScore = 0;
    private int computerScore = 0;
    private GestureClient gestureClient;

    //INICIA EL SCRIPT Y REFERENCIA A GESTURECLIENT
    void Start()
    {
        gestureClient = FindObjectOfType<GestureClient>();
        detectingGestureText.gameObject.SetActive(false); 
        rivalChoiceText.gameObject.SetActive(false); 
    }

    //INICIA EL JUEGO
    public void StartGame()
    {
        if (!gamePanel.activeSelf)
        {
            gamePanel.SetActive(true);
        }
        startMenu.SetActive(false);
        ResetGameVariables(); 
        StartCoroutine(GameRound());
    }

    //LÓGICA DE CADA RONDA
    private IEnumerator GameRound()
    {
        while (currentRound < 5)
        {
            currentRound++;
            roundText.text = $"ROUND {currentRound}";
            yield return new WaitForSeconds(1);

            for (int i = 3; i > 0; i--)
            {
                countDownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }

            countDownText.text = ""; 
            detectingGestureText.gameObject.SetActive(true);
            detectingGestureText.text = "Detecting Gesture...";
            Debug.Log("Detecting Gesture...");

            //SOLICITA LA PREDICCIÓN DEL SERVIDOR Y ESPERA A UN GESTO VÁLIDO
            int playerGestureIndex = -1;
            while (playerGestureIndex == -1)
            {
                yield return StartCoroutine(RequestPlayerGesture());
                playerGestureIndex = gestureClient.GetGesture();
                if (playerGestureIndex == -1)
                {
                    Debug.Log("Waiting for a valid gesture.");
                }
            }
            detectingGestureText.gameObject.SetActive(false);
            Debug.Log("Prediction received.");
            int computerGestureIndex = UnityEngine.Random.Range(0, gestures.Length);
            roundText.gameObject.SetActive(false);
            rivalChoiceText.gameObject.SetActive(true);
            rivalChoiceText.text = "Rival chooses...";
            ActivateGesture(computerGestureIndex);
            yield return new WaitForSeconds(2); 
            rivalChoiceText.gameObject.SetActive(false);
            DeactivateGesture(computerGestureIndex);

            //DETERMINA EL GANADOR
            DetermineWinner(playerGestureIndex, computerGestureIndex);

            yield return new WaitForSeconds(2); 
            roundText.gameObject.SetActive(true);
            DeactivateGesture(computerGestureIndex);

            if (currentRound >= 5)
            {
                ShowFinalResults();
            }
        }
    }
    //SOLICITA LA PREDICCIÓN DEL GESTO AL SERVIDOR
    private IEnumerator RequestPlayerGesture()
    {
        yield return gestureClient.RequestPrediction();
    }
    
    //ACTIVA EL GESTO CORRESPONDIENTE
    private void ActivateGesture(int gestureIndex)
    {
        foreach (var gesture in gestures)
        {
            gesture.SetActive(false); 
        }
        gestures[gestureIndex].SetActive(true);
    }

    //DESACTIVA EL GESTO CORRESPONDIENTE
    private void DeactivateGesture(int gestureIndex)
    {
        gestures[gestureIndex].SetActive(false);
    }

    //MUESTRA LOS RESULTADOS FINALES DEL JUEGO
    private void ShowFinalResults()
    {
        roundText.text = playerScore > computerScore ? "Player wins!" : "Rival wins!";
        StartCoroutine(ResetGame());
    }

    //RESETEA EL JUEGO DESPUÉS DE MOSTRAR LOS RESULTADOS FINALES
    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3);
        startMenu.SetActive(true);
        gamePanel.SetActive(false);
    }

    //RESETEA LAS VARIABLES DEL JUEGO
    private void ResetGameVariables()
    {
        currentRound = 0;
        playerScore = 0;
        computerScore = 0;
        roundText.text = "";
        countDownText.text = "";
        detectingGestureText.gameObject.SetActive(false);
        rivalChoiceText.gameObject.SetActive(false);
    }

    //DETERMINA EL GANADOR DE LA RONDA
    private void DetermineWinner(int playerGesture, int computerGesture)
    {
        if (playerGesture == computerGesture)
        {
            roundText.text = "Tie";
        }
        else
        {
            bool playerWins = (playerGesture == 0 && computerGesture == 1) ||
                              (playerGesture == 1 && computerGesture == 2) ||
                              (playerGesture == 2 && computerGesture == 0);

            if (playerWins)
            {
                playerScore++;
                roundText.text = "Player wins the round!";
            }
            else
            {
                computerScore++;
                roundText.text = "Rival wins the round!";
            }
        }
        roundText.gameObject.SetActive(true);
    }
}
