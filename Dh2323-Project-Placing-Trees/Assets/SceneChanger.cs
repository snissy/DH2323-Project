using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TextClockObject;
    public int nextScene = 1;
    private const string ClockLabel = "Time left: </color>{0:2}";
    private float time = 35.0f;
    private TextMeshProUGUI text;
    
    void Start()
    {
        text = TextClockObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(ClockLabel, time);
        time = time - Time.deltaTime;

        if (time <= 0)
        {
            SceneManager.LoadScene (sceneBuildIndex:nextScene);
        }
    }
}
