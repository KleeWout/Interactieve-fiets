using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Net;
using System.Threading;


//https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10


public class UnityHttpListener : MonoBehaviour
{

    private HttpListener listener;
    private Thread listenerThread;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/");
        // listener.Prefixes.Add("http://*:8080/");
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

        listenerThread = new Thread(startListener);
        listenerThread.Start();
        Debug.Log("Server Started");
    }

    private void OnDestroy()
    {
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            listener.Close();
            listenerThread.Abort();
            Debug.Log("Server Stopped");
        }
    }


    private void startListener()
    {
        while (true)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }


    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);

        Debug.Log("Method: " + context.Request.HttpMethod);
        Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        if (context.Request.QueryString.AllKeys.Length > 0)
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
            }

        if (context.Request.HttpMethod == "GET")
        {
            string responseString = GetResponseBasedOnParameters(context.Request.QueryString);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            context.Response.ContentLength64 = buffer.Length;
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }


        else if (context.Request.HttpMethod == "POST")
        {
            Thread.Sleep(1000);
            var data_text = new StreamReader(context.Request.InputStream,
                                             context.Request.ContentEncoding).ReadToEnd();
            Debug.Log($"Message received: {data_text}");
        }



        context.Response.Close();
    }

    private string GetResponseBasedOnParameters(System.Collections.Specialized.NameValueCollection parameters)
    {
        string name = parameters["name"];
        string age = parameters["age"];

        return "No parameters used but this is just for the test.";
    }
}