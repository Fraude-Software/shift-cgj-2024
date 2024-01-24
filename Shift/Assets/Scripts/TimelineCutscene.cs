using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TimelineCutscene : MonoBehaviour
{
    [SerializeField]
    PlayableDirector playbaleDirector;

    public void Play(float time)
    {
        playbaleDirector.time = time;
        SceneManager.LoadScene("main");
    }
}