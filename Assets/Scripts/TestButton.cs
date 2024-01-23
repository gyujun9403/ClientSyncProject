using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestButton : MonoBehaviour
{
    const float TEST_TIME = 2.0f;

    bool _enabled = false;
    float _testStartTime = 0;
    Timer _timer;
    Button _button;
    TMP_Text _bText;
    GameObject _testButtonObj;
    
    void Start()
    {
        _testButtonObj = GameObject.Find("TestButton");
        _bText = _testButtonObj.GetComponentInChildren<TMP_Text>();
        _button = _testButtonObj.GetComponent<Button>();
        if (_button == null )
        {
            Debug.Log("Button is null");
        }
        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        if (_timer == null)
        {
            Debug.Log("Timer is null");
        }
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(
            delegate {
            ButtonClicked(_button); 
        });
    }

    void Update()
    {
        if (_enabled == true && TEST_TIME + _testStartTime < _timer.GetTime())
        {
            _bText.text = "Test Done";
            _enabled = false;
            GameObject.Find("Logger").GetComponent<Logger>().StopLogging();
            GameObject.Find("Net").GetComponent<NetworkObj>().DisconnectFromServer();
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().StopRunning();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().StopCharacter();
        }
    }

    void ButtonClicked(Button button)
    {
        if (_enabled == false)
        {
            _testStartTime = _timer.GetTime();
            _bText.text = "Testing...";
            _enabled = true;
            GameObject.Find("Net").GetComponent<NetworkObj>().ConnectToServer();
            Debug.Log("ConnectToServer Done");
            GameObject.Find("Logger").GetComponent<Logger>().StartLogging();
            Debug.Log("SetLogging Done");
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().StartRunning();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().MoveCharacter();
        }
    }
}
