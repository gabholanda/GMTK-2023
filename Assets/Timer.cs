using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProGUI timer;

    [Header{"Timer Settings")]
    public float currentTime;
    public bool countDown;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTimer += Timer.deltaTime;
        timerText.text = currenTime.ToString();
    }
}
