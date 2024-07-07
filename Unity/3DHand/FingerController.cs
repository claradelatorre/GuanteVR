using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading;

public class FingerController : MonoBehaviour
{
    // ARRAYS PARA CADA DEDO
    public Transform[] thumbJoints;
    public Transform[] indexJoints;
    public Transform[] middleJoints;
    public Transform[] ringJoints;
    public Transform[] pinkyJoints;

    //OFFSETS PARA CADA DEDO
    private Vector3[] thumbOffsets = new Vector3[] { new Vector3(9f, 156.902f, 37.229f), new Vector3(0f, 0f, -16.368f), new Vector3(0f, 0f, 1f) };
    private Vector3[] indexOffsets = new Vector3[] { new Vector3(-74.875f, 106.624f, 75.113f), new Vector3(0f, 0f, -10.468f), new Vector3(0f, 0f, 0f) };
    private Vector3[] middleOffsets = new Vector3[] { new Vector3(-80.399f, 18.783f, 155.906f), new Vector3(0f, 0f, -10.252f), new Vector3(0f, 0f, -1f) };
    private Vector3[] ringOffsets = new Vector3[] { new Vector3(-69.153f, -21.521f, -163.447f), new Vector3(0f, 0f, -10.414f), new Vector3(0f, 0f, -1.631f) };
    private Vector3[] pinkyOffsets = new Vector3[] { new Vector3(-1.022f, -2.061f, 176.516f), new Vector3(0f, 0f, -8.946f), new Vector3(0f, 0f, -1.387f) };

    //CLIENTE TCP
    private TcpClient client;
    private StreamReader reader;
    private Thread clientThread;
    private string serverIP = "192.168.1.108"; 
    private int port = 12345; 

    //AJUSTAR SENSIBILIDAD
    public float sensitivity = 0.8f;

    void Start()
    {
        //INICIA HILO PARA CONECTARSE AL SERVIDOR
        clientThread = new Thread(new ThreadStart(ConnectToServer));
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    //MÉTODO PARA CONECTARSE AL SERVIDOR
    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, port);
            Stream stream = client.GetStream();
            reader = new StreamReader(stream);
        }
        catch (SocketException e)
        {
            Debug.LogError("socketException: " + e.ToString());
        }
    }

    //VERIFICA SI EL CLIENTE ESTÁ CONECTADO Y HAY DATOS DISPONIBLES
    void Update()
    {
        if (client != null && client.Connected && client.Available > 0)
        {
            string dataString = reader.ReadLine();
            if (!string.IsNullOrEmpty(dataString))
            {
                ProcessData(dataString);
            }
        }
    }

    //PROCESA LOS DATOS RECIBIDOS
    void ProcessData(string dataString)
    {
        string[] sensorData = dataString.Split(',');
        if (sensorData.Length >= 5)
        {
            ProcessFingerData(float.Parse(sensorData[0]), thumbJoints, thumbOffsets);
            ProcessFingerData(float.Parse(sensorData[1]), indexJoints, indexOffsets);
            ProcessFingerData(float.Parse(sensorData[2]), middleJoints, middleOffsets);
            ProcessFingerData(float.Parse(sensorData[3]), ringJoints, ringOffsets);
            ProcessFingerData(float.Parse(sensorData[4]), pinkyJoints, pinkyOffsets);
        }
        else
        {
            Debug.LogError("error: no se han recibido datos suficientes");
        }
    }

  
    //PROCESA LOS DATOS DE ROTACIÓN DE UN DEDO ESPECÍFICO
    void ProcessFingerData(float sensorValue, Transform[] joints, Vector3[] offsets)
    {
        float rotationAngle = MapSensorValueToRotation(sensorValue);
        ApplyRotationToJoints(rotationAngle, joints, offsets);
    }

    //MAPEA EL VALOR DEL SENSOR A UN ÁNGULO DE ROTACIÓN
    private float MapSensorValueToRotation(float sensorValue)
    {
        return (1023 - sensorValue) / 1023f * 90f;
    }

    //APLICA LA ROTACIÓN A LAS ARTICULACIONES DEL DEDO
    private void ApplyRotationToJoints(float rotationAngle, Transform[] joints, Vector3[] offsets)
    {
        float adjustedAngle = rotationAngle * sensitivity;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].localRotation = Quaternion.Euler(offsets[i]) * Quaternion.Euler(0f, 0f, -adjustedAngle);
        }
    }

    private void OnDestroy()
    {
        if (reader != null)
        {
            reader.Close();
        }
        if (client != null)
        {
            client.Close();
        }
        if (clientThread != null)
        {
            clientThread.Abort();
        }
    }
}
