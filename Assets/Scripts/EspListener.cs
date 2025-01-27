using UnityEngine;
using System.Collections;
using System.Globalization;

public class EspListener : MonoBehaviour
{

    public float valueLeft;
    public float valueRight;

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        string[] msgSplit = msg.Split(',');
        string message1 = msgSplit[0];
        string message2 = msgSplit[1];
    CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");
    valueLeft = float.Parse(message1, en_us);
    valueRight = float.Parse(message2, en_us);
}


    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
    }
}
