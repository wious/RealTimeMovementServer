using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public class NetworkedServer : MonoBehaviour
{
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 9003;

    LinkedList<MsgToSendWithLatency> msgsToSendWithLatency;

    void Start()
    {
        if (NetworkedServerProcessing.GetNetworkedServer() == null)
        {
            NetworkedServerProcessing.SetNetworkedServer(this);

            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, socketPort, null);

            msgsToSendWithLatency = new LinkedList<MsgToSendWithLatency>();
        }
        else
        {
            Debug.Log("Singleton-ish architecture violation detected, investigate where NetworkedServer.cs Start() is being called.  Are you creating a second instance of the NetworkedServer game object or has the NetworkedServer.cs been attached to more than one game object?");
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                NetworkedServerProcessing.ConnectionEvent(recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                NetworkedServerProcessing.ReceivedMessageFromClient(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                NetworkedServerProcessing.DisconnectionEvent(recConnectionID);
                break;
        }

        #region Simulated Latency

        LinkedList<MsgToSendWithLatency> processMe = null;


        foreach (MsgToSendWithLatency m in msgsToSendWithLatency)
        {
            m.latency -= Time.deltaTime;

            if (m.latency <= 0)
            {
                if (processMe == null)
                    processMe = new LinkedList<MsgToSendWithLatency>();

                processMe.AddLast(m);
            }
        }

        if (processMe != null)
        {
            foreach (MsgToSendWithLatency m in processMe)
            {
                msgsToSendWithLatency.Remove(m);
                SendMessageToClient(m.msg, m.id);
            }
        }

        #endregion

    }
    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    public void SendMessageToClientWithSimulatedLatency(string msg, int id)
    {
        msgsToSendWithLatency.AddLast(new MsgToSendWithLatency(msg, id));
    }

}

class MsgToSendWithLatency
{
    public string msg;
    public int id;
    const float LatencySimulationTotal = 0.5f;
    public float latency = LatencySimulationTotal;

    public MsgToSendWithLatency(string Msg, int ID)
    {
        msg = Msg;
        id = ID;
    }

}