using UnityEngine;

public class ServerConnToggle : MonoBehaviour
{
    public UnityEngine.UI.Toggle _toggle;
    
    NetworkObj _netObj;
    Logger _logger;

    void Start()
    {
        _toggle = GetComponent<UnityEngine.UI.Toggle>();
        _netObj = GameObject.Find("Net").GetComponent<NetworkObj>();
        if (_netObj == null)
        {
            Debug.Log("Net is null");
        }
        _logger = GameObject.Find("Logger").GetComponent<Logger>();
        if (_logger == null)
        {
            Debug.Log("Logger is null");
        }
        _toggle = GetComponent<UnityEngine.UI.Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().StartRunning();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            _netObj.ConnectToServer();
            _logger.StartLogging();
            Debug.Log("ServerConnToggle On");
        }
        else
        {
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().StopRunning();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            _netObj.DisconnectFromServer();
            _logger.StopLogging();
            Debug.Log("ServerConnToggle Off");
        }
    }
}
