using System;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public float _loggingStartTime;
    
    static int _thisLoggingCnt = 0;
    static int _otherLoggingCnt = 0;
    bool _isLogging = false;

    Timer _timer;
    StreamWriter _swThis = null;
    StreamWriter _swOther = null;

    async void Start()
    {
        // 현재 시간을 바탕으로 로그 파일 명 생성
        string logThisFileName = "clientLog_this_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        string logOtherFileName = "clientLog_other_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        _swThis = new StreamWriter("C:\\Users\\gujun\\Documents\\UnitySyncLog\\" + logThisFileName, true);
        _swOther = new StreamWriter("C:\\Users\\gujun\\Documents\\UnitySyncLog\\" + logOtherFileName, true);

        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        if (_timer == null)
        {
            Debug.Log("Timer is null");
        }

        await _swThis.WriteLineAsync("Time\tLocX\tLocY\tVelX\tVelY");
        await _swOther.WriteLineAsync("Time\tLocX\tLocY\tVelX\tVelY");
    }

    public async void WriteLogThis(float locX, float locY, float velX, float velY)
    {
        if (_isLogging == false)
        {
            return;
        }

        DateTime currentTime = System.DateTime.Now;
        await _swThis.WriteLineAsync(string.Format("{0:0.000}\t{1:0.000}\t{2:0.000}\t{3:0.000}\t{4:0.000}", currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), locX, locY, velX, velY));
        
        if (_thisLoggingCnt++ == 5)
        {
            _thisLoggingCnt = 0;
            await _swThis.FlushAsync();
        }
    }

    public async void WriteLogOther(float locX, float locY, float velX, float velY)
    {
        if (_isLogging == false)
        {
            return;
        }

        DateTime currentTime = System.DateTime.Now;
        await _swOther.WriteLineAsync(string.Format("{0:0.000}\t{1:0.000}\t{2:0.000}\t{3:0.000}\t{4:0.000}", currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), locX, locY, velX, velY));
        
        if (_otherLoggingCnt++ == 5)
        {
            _otherLoggingCnt = 0;
            await _swOther.FlushAsync();
        }
    }

    public float GetLoggingTime()
    {
        return _timer.GetTime() - _loggingStartTime;
    }

    public void StartLogging()
    {
        _loggingStartTime = _timer.GetTime();
        _isLogging = true;
    }

    public void StopLogging()
    {
        _isLogging = false;
    }

    public void OnDestroy()
    {
        Debug.Log("Log OnDestroy()");
        _swThis.FlushAsync();
        _swOther.FlushAsync();
        _swThis.Close();
        _swOther.Close();
    }
}
