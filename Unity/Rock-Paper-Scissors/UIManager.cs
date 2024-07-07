using UnityEngine;

public class UIManager : MonoBehaviour
{
    //PANELES DEL JUEGO
    public GameObject startPanel; 
    public GameObject gamePanel; 
    public GameObject instructionsPanel; 

    //MUESTRA EL PANEL DE INICIO DEL JUEGO
    void Start()
    {
        ShowStartPanel(); 
    }

    //MUESTRA EL PANEL DE JUEGO Y OCULTA LOS DEMÁS
    public void ShowGamePanel()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        instructionsPanel.SetActive(false);
    }

    // MUESTRA LAS INSTRUCCIONES Y OCULTA LOS DEMÁS PANELES
    public void ShowInstructionsPanel()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    //MUESTRA EL PANEL DE INICIO Y OCULTA LOS DEMÁS (PARA REGRESAR)
    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        instructionsPanel.SetActive(false);
    }

    
}
