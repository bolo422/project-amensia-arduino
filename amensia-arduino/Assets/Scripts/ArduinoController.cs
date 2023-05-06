using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;
using System;

public class ArduinoController : MonoBehaviour
{
    Thread IOThread = new Thread(DataTread);
    private static SerialPort sp;

    private static string incomingMsg = "";
    private static string outgoingMsg = "";
    
    private static bool isArduinoActivated = true;

    private static int dial, volt, ldr;

    private static void DataTread()
    {
        sp = new SerialPort("COM4", 9600);
        sp.Open();

        while (true)
        {
            if (outgoingMsg != "")
            {
                sp.Write(outgoingMsg);
                outgoingMsg = "";
            }
            incomingMsg = sp.ReadExisting();
            Thread.Sleep(200);
        }
    }

    private void OnDestroy()
    {
        IOThread.Abort();
        sp.Close();
    }

    private void Start()
    {
        isArduinoActivated = GameManager.Instance.activateArduino;
        if(!isArduinoActivated) return;
    }

    private void Update()
    {
        if(!isArduinoActivated) return;
        
        if (incomingMsg != "")
        {
            string[] parts = incomingMsg.Split(';');

            dial = int.Parse(parts[1]);
            volt = int.Parse(parts[3]);
            ldr = int.Parse(parts[5]);
            /*
             
             
             
             */
            GameManager.Instance.UpdateArduinoInput(dial, volt, ldr);
        }

    }


}
