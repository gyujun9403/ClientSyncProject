using System.Collections.Concurrent;
using UnityEngine;
using TMPro;

public struct StatusData
{
    public float locX;
    public float locY;
    public float velX;
    public float velY;
    public float ttsThis;
    public float ttsOther;
    public float displayTime;

    public StatusData(float inputLocX, float inputLocY, float inputVelX, float inputVelY)
    {
        locX = inputLocX;
        locY = inputLocY;
        velX = inputVelX;
        velY = inputVelY;
        ttsThis = 0;
        ttsOther = 0;
        displayTime = 0;
    }
}

public class UserStatusDelayQueue : MonoBehaviour
{
    public bool networkConnected = false;
    
    float _sendDelay = 0f;
    float _recvDelay = 0f;
    Timer _timer = null;
    TMP_InputField _sendDelayInput;
    TMP_InputField _recvDelayInput;
    ConcurrentQueue<StatusData> statusSendQueue = new ConcurrentQueue<StatusData>();
    ConcurrentQueue<StatusData> statusRecvQueue = new ConcurrentQueue<StatusData>();
    
    void Start()
    {
        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        if (_timer == null)
        {
            Debug.Log("Timer is null");
        }

        _sendDelayInput = GameObject.Find("SendDelayInput").GetComponent<TMP_InputField>();
        if (_sendDelayInput == null)
        {
            Debug.Log("SendDelayInput is null");
        }

        _recvDelayInput = GameObject.Find("RecvDelayInput").GetComponent<TMP_InputField>();
        if (_recvDelayInput == null)
        {
            Debug.Log("RecvDelayInput is null");
        }
    }

    void Update()
    {
        _sendDelay = 0.001f * float.Parse(_sendDelayInput.text);
        _recvDelay = 0.001f * float.Parse(_recvDelayInput.text);
    }

    public void SetSendDelay(float delay)
    {
        _sendDelay = delay;
    }

    public void SetRecvDelay(float delay)
    { 
        _recvDelay = delay;
    }

    public void SendEnqueue(StatusData status)
    {
        if (networkConnected == false)
        {
            return;
        }
        status.displayTime = _timer.GetTime() + _sendDelay;
        status.ttsThis = _sendDelay;
        status.ttsOther = _recvDelay;
        statusSendQueue.Enqueue(status);
    }

    public StatusData? TrySendDequeue()
    {
        if (statusSendQueue.TryPeek(out StatusData temp) == true && temp.displayTime <= _timer.GetTime())
        {
            while (statusSendQueue.TryDequeue(out StatusData garbege) == false)
            {
                ;
            }
            return temp;
        }
        return null;
    }

    public void RecvEnqueue(StatusData status)
    {
        if (networkConnected == false)
        {
            return;
        }
        status.displayTime = _timer.GetTime() + _recvDelay;
        status.ttsThis = _sendDelay;
        status.ttsOther = _recvDelay;
        statusRecvQueue.Enqueue(status);
    }

    public StatusData? TryRecvDequeue()
    {
        if (statusRecvQueue.TryPeek(out StatusData temp) == true && temp.displayTime <= _timer.GetTime())
        {
            while (statusRecvQueue.TryDequeue(out StatusData garbege) == false)
            {
                ;
            }
            return temp;
        }
        return null;
    }
}
