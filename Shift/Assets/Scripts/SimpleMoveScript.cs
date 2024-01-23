using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SimpleMoveScript : MonoBehaviour
{
    private Collider2D col;
    private CinemachineBrain brain;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();

        brain = FindObjectOfType<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(new Vector2(0.1f, 0.0f));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //get parent of other
        LevelData levelData = other.GetComponent<LevelData>();

        //get vcam from parent
        CinemachineVirtualCamera vcam = levelData.VCam;

        vcam.Follow = this.transform;

        //blend between current camera and vcam
        brain.ActiveVirtualCamera.Priority = 10;
        vcam.GetComponent<CinemachineVirtualCamera>().Priority = 11;
    }
}
