using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class MPUController : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    public string serverIP = "192.168.1.108";
    public int port = 12345;

    //POSICIÓN INICIAL DE LA MANO
    private Vector3 initialRotation = new Vector3(0f, 0f, 0f);

    //OFFSET INICIAL 
    private Vector3 gyroOffset = new Vector3(68f, -48f, 88f);
    
    //AJUSTAR SENSIBILIDAD
    private float scaleFactor = 0.1f;


    void Start()
    {
        //ROTACIÓN INICIAL DE LA MANO
        transform.localRotation = Quaternion.Euler(initialRotation);
        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            Debug.Log("conexión establecida");
        }
        catch (System.Exception e)
        {
            Debug.LogError("error al conectar: " + e.Message);
        }
    }

    //VERIFICA SI EL CLIENTE ESTÁ CONECTADO, SI HAY DATOS DISPONIBLES Y PROCESA LOS DATOS
    void Update()
    {
        if (client != null && client.Connected && stream != null && stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            ProcessGyroscopeData(receivedData);
        }
    }

    void ProcessGyroscopeData(string data)
    {
        string[] splitData = data.Split(',');
        if (splitData.Length == 3)
        {
            float rawGyroX = float.Parse(splitData[0]);
            float rawGyroY = float.Parse(splitData[1]);
            float rawGyroZ = float.Parse(splitData[2]);

            //APLICA EL OFFSET
            float gyroX = rawGyroX - gyroOffset.x;
            float gyroY = rawGyroY - gyroOffset.y;
            float gyroZ = rawGyroZ - gyroOffset.z;
            Debug.Log($"Corrected Gyroscope data - X: {gyroX}, Y: {gyroY}, Z: {gyroZ}");

            //MAPEO DE EJES
            float correctedGyroX = gyroY; 
            float correctedGyroY = -gyroZ; 
            float correctedGyroZ = -gyroX;

            //CREA UN VECTOR DE LA VELOCIDAD ANGULAR Y ELIMINA RUIDO
            Vector3 angularVelocity = new Vector3(
                (Mathf.Abs(correctedGyroX) > 2) ? correctedGyroX : 0,
                (Mathf.Abs(correctedGyroY) > 2) ? correctedGyroY : 0,
                (Mathf.Abs(correctedGyroZ) > 2) ? correctedGyroZ : 0
            );

           //AJUSTA LA SENSIBILIDAD DE LA ROTACIÓN
            angularVelocity *= scaleFactor;

            //CALCULA LA ROTACIÓN
            if (angularVelocity != Vector3.zero)
            {
                Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.deltaTime);
                transform.localRotation *= deltaRotation;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}
