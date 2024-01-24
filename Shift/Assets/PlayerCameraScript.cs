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

    void Update()
    {
        var collider = GetComponent<Collider2D>();
        //list all colliders that are touching this collider
        var colliders = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0.0f);
        
        foreach (var other in colliders)
        {
            LevelData levelData = other.GetComponent<LevelData>();

            if (levelData != null)
            {
                CinemachineVirtualCamera vcam = levelData.VCam;

                vcam.Follow = this.transform;

                brain.ActiveVirtualCamera.Priority = 10;
                vcam.GetComponent<CinemachineVirtualCamera>().Priority = 11;

                playerMovement.ChangeLevel(levelData);
                return;
            }
        }
    }
}
