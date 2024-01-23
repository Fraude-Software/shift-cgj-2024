using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        vcam = transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public CinemachineVirtualCamera VCam {
        get{
            return vcam;
        }
    }
}
