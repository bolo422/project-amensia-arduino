using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Windows;
using System;
using Microsoft.Win32;

public class ArduinoController : MonoBehaviour
{
    Thread IOThread = new Thread(DataTread);
    private static SerialPort sp;

    private static string incomingMsg = "";
    private static string outgoingMsg = "";

    private static string arduinoName = "CH340";
    
    //private static bool isArduinoActivated = true;

    private static int dial, volt, ldr;

    public static string port;

    private static void DataTread()
    {
        sp = new SerialPort(port, 9600);
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
        //isArduinoActivated = GameManager.Instance.activateArduino;
        //if(!isArduinoActivated) return;
        port = AutodetectArduinoPort();
        if (port != null)
        {
            IOThread.Start();
            GameManager.Instance.ActivateArduino = true;
        }
        else
            GameManager.Instance.ActivateArduino = false;
        
    }

    private void Update()
    {
        //if(!isArduinoActivated) return;
        if (incomingMsg != "")
        {
            // Debug.Log(incomingMsg);
            string[] parts = incomingMsg.Split(';');

            dial = int.Parse(parts[1]);
            volt = int.Parse(parts[3]);
            ldr = int.Parse(parts[5]);
            
            // Debug.Log(dial);
            
            GameManager.Instance.UpdateArduinoInput(dial, volt, ldr);
        }

    }

    public static string AutodetectArduinoPort()
    {
        List<string> comports = new List<string>();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;
        string debugS3 = "debugS3: ";
        string debugS2 = "debugS2: ";
        string debugS = "debugS: ";
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            debugS3 += s3 + ", ";
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                debugS += s + ", ";
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        debugS2 += s2 + ", ";
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        // Debug.Log("temp: " + (string)rk5.GetValue("FriendlyName"));
                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null && temp.Contains(arduinoName))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
            // Debug.Log(debugS);
            // Debug.Log(debugS2);
            // Debug.Log(debugS3);
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }

        return null;
    }
}
