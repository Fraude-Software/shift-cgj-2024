using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    private CinemachineBrain brain;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        brain = FindObjectOfType<CinemachineBrain>();
    }

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        LevelData levelData = other.GetComponent<LevelData>();
        playerMovement.CurrentLevelData = levelData;
        CinemachineVirtualCamera vcam = levelData.VCam;
        vcam.Follow = this.transform;
        brain.ActiveVirtualCamera.Priority = 10;
        vcam.Priority = 11;
    }
}
