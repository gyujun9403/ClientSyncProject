using UnityEngine;

public class InputDelayToggle : MonoBehaviour
{
    public UnityEngine.UI.Toggle _toggle;
    
    ThisClientCharacter _thisCharacter;
    
    void Start()
    {
        _toggle = GetComponent<UnityEngine.UI.Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        _thisCharacter = GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>();
        if (_thisCharacter == null)
        {
            Debug.Log("Charactor01 is null");
        }
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            _thisCharacter._delayEnabled = true;
            Debug.Log("InputDelayToggle On");
        }
        else
        {
            GameObject.Find("player1D_0_Other").GetComponent<OtherClientView>().ResetChracter();
            GameObject.Find("Charactor01").GetComponent<ThisClientCharacter>().ResetChracter();
            _thisCharacter._delayEnabled = false;
            Debug.Log("InputDelayToggle Off");
        }
    }
}
