using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientConnectionID);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.KeyboardInputUpdate)
        {
            int directionalInput = int.Parse(csv[1]);
            gameLogic.GetComponent<GameLogic>().UpdateDirectionalInput(directionalInput, clientConnectionID);
        }
        // else if (signifier == ClientToServerSignifiers.asd)
        // {

        // }

        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    static public void SendMessageToClientWithSimulatedLatency(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClientWithSimulatedLatency(msg, clientConnectionID);
    }

    
    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers


static public class ServerToClientSignifiers
{
    public const int VelocityAndPosition = 1;
}
static public class ClientToServerSignifiers
{
    public const int KeyboardInputUpdate = 1;
}


static public class KeyboardInputDirections
{
    public const int Up= 1;
    public const int Down = 2;
    public const int Left = 3;
    public const int Right= 4;

    public const int UpRight = 5;
    public const int UpLeft = 6;
    public const int DownRight = 7;
    public const int DownLeft = 8;
    public const int NoPress = 100;
}
#endregion