using System;
using System.Net.Sockets;
using UnityEngine;

public class NetworkObj : MonoBehaviour
{
    const int BUFFER_SIZE = 512;
    
    public bool isConnected = false;
    
    const string IP = "127.0.0.1";
    const int PORT = 3334;
    byte[] _recvBuf = null;
    Logger _logger;
    NetworkStream _stream;
    TcpClient _tcpClient;
    UserStatusDelayQueue _userStatusDelayQueue;

    void Start()
    {
        _tcpClient = null;
        _stream = null;

        _recvBuf = new byte[BUFFER_SIZE];

        _userStatusDelayQueue = GameObject.Find("UserStatusDelayQueue").GetComponent<UserStatusDelayQueue>();
        if (_userStatusDelayQueue == null)
        {
            Debug.Log("UserStatusDelayQueue is null");
        }

        _logger = GameObject.Find("Logger").GetComponent<Logger>();
        if (_logger == null)
        {
            Debug.Log("Logger is null");
        }
    }

    async void Update()
    {
        if (isConnected == false)
        {
            return;
        }

        try
        {
            StatusData? _statusData = _userStatusDelayQueue.TrySendDequeue();
            if (_statusData != null)
            {
                CharaterStatReq req = new CharaterStatReq(
                    _statusData.Value.locX, _statusData.Value.locY,
                    _statusData.Value.velX, _statusData.Value.velY,
                    _logger.GetLoggingTime(),
                    _statusData.Value.ttsThis, _statusData.Value.ttsOther);
                await _stream.WriteAsync(req.Serialize(), 0, req.GetPacketLength());
            }

            int bytesRead = await _stream.ReadAsync(_recvBuf, 0, _recvBuf.Length);
            if (bytesRead > 0)
            {
                CharaterStatRes res = new CharaterStatRes();
                res.Deserialize(_recvBuf);
                StatusData statusData = new StatusData();
                statusData.locX = res.locX; statusData.locY = res.locY;
                statusData.velX = res.velX; statusData.velY = res.velY;
                statusData.ttsThis = res.ttsThis; statusData.ttsOther = res.ttsOther;
                _userStatusDelayQueue.RecvEnqueue(statusData);
            }
            else
            {
                // 연결이 끊어진 경우
                isConnected = false;
                Debug.Log("Connection to server closed.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    public void ConnectToServer()
    {
        if (isConnected == true)
        {
            return;
        }

        try
        {
            _tcpClient = new TcpClient(IP, PORT);
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = 1;
            isConnected = true;
            _userStatusDelayQueue.networkConnected = true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void DisconnectFromServer()
    {
        if (isConnected == false)
        {
            return;
        }

        try
        {
            isConnected = false;
            _userStatusDelayQueue.networkConnected = false;
            _stream.Close();
            if (_tcpClient != null)
            {
                _tcpClient.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void OnDestroy()
    {
        isConnected = false;
        // 스크립트가 파괴될 때 연결을 종료
        if (_stream != null)
        {
            _stream.Close();
        }
        if (_tcpClient != null)
        {
            _tcpClient.Close();
        }
    }
}
