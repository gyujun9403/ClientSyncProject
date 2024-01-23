using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

struct reservedStatus
{
    public float reservedTime;
    public Vector3 reservedLoc;
    public Vector3 reservedVel;

    public reservedStatus(float reservedTime = 0, Vector3 reservedLoc = default(Vector3), Vector3 reservedVel = default(Vector3))
    {
        this.reservedTime = reservedTime;
        this.reservedLoc = reservedLoc;
        this.reservedVel = reservedVel;
    }
}

public class ThisClientCharacter : MonoBehaviour
{
    const float Z = -10f;
    const float SPEED = 10;

    public Vector3 _speed = Vector3.zero;
    public Vector3 _manualPreSpeed = Vector3.zero;
    public bool _autoMoving = false;
    public bool _delayEnabled = false;

    float _fCnt = 0;
    float _nextSendTime;
    float _thisClientDelay;
    float _otherClientDelay;

    Timer _timer;
    Logger _logger;
    GameObject _character;
    Rigidbody2D g_CaractorRbody;
    UserStatusDelayQueue _userStatusDelayQueue;
    TMP_InputField _sendDelayInput;
    TMP_InputField _recvDelayInput;
    // 예약시간, 위치, 속도 순 저장.
    LinkedList<reservedStatus> _statusReserveList = new LinkedList<reservedStatus>();

    void Start()
    {
        g_CaractorRbody = GetComponent<Rigidbody2D>();
        g_CaractorRbody.gravityScale = 0;
        g_CaractorRbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        _character = GameObject.Find("Charactor01");
        if (_character == null)
        {
            Debug.Log("Charactor01 is null");
        }

        _logger = GameObject.Find("Logger").GetComponent<Logger>();
        if (_logger == null)
        {
            Debug.Log("Logger is null");
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

        _nextSendTime = 0.001f * 50f;
        _thisClientDelay = 0.001f * float.Parse(_sendDelayInput.text);
        _otherClientDelay = 0.001f * float.Parse(_recvDelayInput.text);
    }

    private void Update()
    {
        AddReserveStatus();
        CheckAndSendTime(_statusReserveList.Last());
        
        // 예약된 상태 시간 확인 후 적용.
        while (_statusReserveList.Count > 0)
        {
            // 첫 항목의 예약시간이 지금보다 미래면 뒤에건 볼 필요도 없으므로 break;
            if (_statusReserveList.First.Value.reservedTime > _timer.GetTime())
            {
                break;
            }
            _speed.x = _statusReserveList.First.Value.reservedVel.x;
            _speed.y = _statusReserveList.First.Value.reservedVel.y;
            _character.transform.position = new Vector3(
                _statusReserveList.First.Value.reservedLoc.x,
                _statusReserveList.First.Value.reservedLoc.y, Z);
            _statusReserveList.RemoveFirst();
        }

        _logger.WriteLogThis(_character.transform.position.x, _character.transform.position.y, _speed.x, _speed.y);
    }

    private void AddReserveStatus()
    {
        reservedStatus reserve = new reservedStatus();

        // 동작 지연 동기화 적용 유무에 따라 상태 갱신 방식 변경
        if (_delayEnabled == true)
        {
            reservedStatus lastReserve = _statusReserveList.LastOrDefault();
            float delay = (_thisClientDelay + _otherClientDelay > 0.03f) ? (0.03f) : (_thisClientDelay + _otherClientDelay);

            reserve.reservedTime = _timer.GetTime() + delay;
            reserve.reservedLoc = new Vector3(
                lastReserve.reservedLoc.x + lastReserve.reservedVel.x * Time.deltaTime,
                lastReserve.reservedLoc.y + lastReserve.reservedVel.y * Time.deltaTime, Z);
        }
        else
        {
            reserve.reservedTime = 0f;
            reserve.reservedLoc = new Vector3(
            _character.transform.position.x + _speed.x * Time.deltaTime,
            _character.transform.position.y + _speed.y * Time.deltaTime, Z);
        }

        // 자동 이동 유무에 따라 속도 예약 변경
        if (_autoMoving == true)
        {
            reserve.reservedVel = new Vector3(
                SPEED * Mathf.Sin(_fCnt * 360 * Mathf.Deg2Rad),
                SPEED * Mathf.Cos(_fCnt * 360 * Mathf.Deg2Rad), 0);
            _fCnt += 0.002f;
        }
        else
        {
            _fCnt = 0f;
            float keyx = Input.GetAxisRaw("Horizontal");
            float keyy = Input.GetAxisRaw("Vertical");
            reserve.reservedVel = new Vector3(keyx, keyy, 0).normalized * SPEED;
        }

        _statusReserveList.AddLast(reserve);
    }

    private void CheckAndSendTime(reservedStatus reserve)
    {
        if (_timer.GetTime() > _nextSendTime)
        {
            StatusData status = new StatusData(
                reserve.reservedLoc.x, reserve.reservedLoc.y,
                reserve.reservedVel.x, reserve.reservedVel.y);
            _userStatusDelayQueue.SendEnqueue(status);
            _nextSendTime = _timer.GetTime() + 0.001f * 50;
        }
    }

    public void MoveCharacter()
    {
        _autoMoving = true;
        _thisClientDelay = 0.001f * float.Parse(_sendDelayInput.text);
        _otherClientDelay = 0.001f * float.Parse(_recvDelayInput.text);
    }

    public void StopCharacter()
    {
        _autoMoving = false;
    }

    public void ResetChracter()
    {
        _speed = Vector3.zero;
        _character.transform.position = Vector3.zero;
        _fCnt = 0;
        _nextSendTime = _timer.GetTime();
        _statusReserveList.Clear();
    }
}
