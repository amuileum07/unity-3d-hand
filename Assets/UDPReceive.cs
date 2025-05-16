using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[Serializable]
public class HandData
{
    public float[] Left;
    public float[] Right;
}

public class UDPReceive : MonoBehaviour
{

    Thread receiveThread;
    UdpClient client; 
    public int port = 5052;
    public bool startRecieving = true;
    public bool printToConsole = false;
    
    public HandData handData;

    public void Start()
    {

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (startRecieving)
        {

            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                string dataStr = Encoding.UTF8.GetString(dataByte);

                if (printToConsole) { print(dataStr); }

                handData = JsonUtility.FromJson<HandData>(dataStr);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

}