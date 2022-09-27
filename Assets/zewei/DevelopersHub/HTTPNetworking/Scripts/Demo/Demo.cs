using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevelopersHub.HTTPNetworking;
using DevelopersHub.HTTPNetworking.Tools;

public class Demo : MonoBehaviour
{

    public Button btnConnect = null;

    private void Start()
    {
        
        Networking.OnRequestResponded += Instance_OnRequestResponded;
        Networking.OnConnectedToServerSuccessful += Networking_OnConnectedToServerSuccessful;
        Networking.OnConnectToServerFailed += Networking_OnConnectToServerFailed;
        Networking.OnDisconnectedFromServer += Networking_OnDisconnectedFromServer;
        btnConnect.onClick.AddListener(ConnectToServer);
    }

    private void ConnectToServer()
    {
        if (Networking.IsConnectedToServer)
        {
            MessageBox.ShowMessage("Reminder!", "You already connected to the server.", MessageBox.Type.info, "OK");
        }
        else
        {
            Loading.Show();
            Networking.ConnectToServer();
        }
    }

    private void SendRequest()
    {
        // Id will be used to identify the request on the server
        int id = 1;

        // You can pass any type of data to the server using a dictionary
        Dictionary<string, object> data = new Dictionary<string, object>();

        // This is how you assign values
        data.Add("key", "value");

        // And finally you send your request and wait for a response
        Networking.SendRequest(id, data);
    }

    private void Networking_OnConnectedToServerSuccessful()
    {
        MessageBox.ShowMessage("Done!", "You connected to server successfully.", MessageBox.Type.success, "OK");
    }

    private void Networking_OnConnectToServerFailed(string error)
    {
        MessageBox.ShowMessage("Failed!", "Could not connect to the server.", MessageBox.Type.error, "OK");
    }

    private void Networking_OnDisconnectedFromServer(string error)
    {
        MessageBox.ShowMessage("Error!", "You lost your connection with the server.", MessageBox.Type.error, "OK");
    }

    private void Instance_OnRequestResponded(int requestID, bool successful, string error, JsonData data)
    {
        // This is where you receive the response to your requests
        if (successful)
        {
            if (data != null)
            {
                Debug.Log(data.ToJson());
            }
        }
        else
        {
            Debug.Log(error);
        }
    }

}