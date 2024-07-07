using UnityEngine;
using System.Collections;

public class InstructionsButtonScript : MonoBehaviour
{
    public GameObject instructionsPanel;  

    //ACTIVA EL PANEL DE INSTRUCCIONES Y COMIENZA CON LA CORRUTINA PARA OCULTARLAS
    public void ToggleInstructions()
    {
        instructionsPanel.SetActive(true); 
        StartCoroutine(HideInstructions());  
    }
    //A LOS 15 SEGUNDOS SE DESACTIVA EL PANEL DE INSTRUCCIONES Y SE OCULTA
    private IEnumerator HideInstructions()
    {
        yield return new WaitForSeconds(15);  
        instructionsPanel.SetActive(false);  
    }
}
