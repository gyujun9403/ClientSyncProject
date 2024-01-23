using UnityEngine;

public class Timer : MonoBehaviour
{
    float _timer = 0;
    
    void Update()
    {
        _timer += Time.deltaTime;
    }

    public float GetTime()
    {
        return _timer;
    }
}
