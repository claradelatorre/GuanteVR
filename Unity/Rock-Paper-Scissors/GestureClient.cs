using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class GestureClient : MonoBehaviour
{
    //CLIENTE HTTP ESTÁTICO PARA REALIZAR PETICIONES
    private static readonly HttpClient client = new HttpClient();
    //INICIALIZA GESTO RECIBIDO
    private int gesture = -1;

    //VACÍO, NO SE NECESITA SOCKET
    void Start()
    {
    
    }

    //SOLICITA LA PREDICCIÓN AL SERVIDOR
    public async Task RequestPrediction()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:5000/get_prediction");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var json = JsonUtility.FromJson<PredictionResponse>(responseBody);
            gesture = json.prediction;
            Debug.Log("Gesture received: " + gesture);
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
    }

    //SE OBTIENE EL VALOR DEL GESTO ACTUAL
    public int GetGesture()
    {
        return gesture;
    }

    void OnDestroy()
    {
        if (client != null)
        {
            client.Dispose();
        }
    }
}

//REPRESENTA LA RESPUESTA DE PREDICCIÓN DEL SERVIDOR
[Serializable]
public class PredictionResponse
{
    public int prediction;
}
