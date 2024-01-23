using UnityEngine;

public class OtherClientView : MonoBehaviour
{
    const float Z = -10f;
    
    public Vector3 _speed;
    
    Timer _timer;
    Logger _logger;
    GameObject otherClientCharacter; 
    UserStatusDelayQueue _userStatusDelayQueue;
    bool _isRunning = false;

    void Start()
    {
        otherClientCharacter = GameObject.Find("player1D_0_Other");
        if (otherClientCharacter == null)
        {
            Debug.Log("otherClientCharacter is null");
        }

        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        if (_timer == null)
        {
            Debug.Log("Timer is null");
        }

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

        _speed = Vector3.zero;
        otherClientCharacter.transform.position = new Vector3(0f, 0f, Z);
    }

    void Update()
    {
        if (_isRunning == false)
        {
            return;
        }

        StatusData? chracterStatus = _userStatusDelayQueue.TryRecvDequeue();
        if (chracterStatus != null)
        {
            _speed.x = chracterStatus.Value.velX;
            _speed.y = chracterStatus.Value.velY;
            otherClientCharacter.transform.position = new Vector3(chracterStatus.Value.locX, chracterStatus.Value.locY, Z);
        }
        else
        {
            // µ¥µå·¹Ä¿´×
            float newX = otherClientCharacter.transform.position.x + _speed.x * Time.deltaTime;
            float newY = otherClientCharacter.transform.position.y + _speed.y * Time.deltaTime;
            otherClientCharacter.transform.position = new Vector3(newX, newY, Z);
        }
        _logger.WriteLogOther(transform.position.x, transform.position.y, _speed.x, _speed.y);
    }

    public void StartRunning()
    {
        _isRunning = true;
    }

    public void StopRunning()
    {
        _isRunning = false;
    }

    public void ResetChracter()
    {
        _speed = Vector3.zero;
        transform.position = Vector3.zero;
    }
}
