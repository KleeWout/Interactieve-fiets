using UnityEngine;
using System.Collections;

public class EspListener : MonoBehaviour
{

    public float valueLeft;
    public float valueRight;

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        Debug.Log("Message arrived: " + msg);
        // string[] msgSplit = msg.Split(',');
        // valueLeft = float.Parse(msgSplit[0]);
        // valueRight = float.Parse(msgSplit[1]);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }
}
