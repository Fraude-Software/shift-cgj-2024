using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelData : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    private GameObject etherMap;
    private GameObject normalMap;
    private Transform playerSpawn;

    public GameObject EtherMap {
        get{
            return etherMap;
        }
    }

    public GameObject NormalMap {
        get{
            return normalMap;
        }
    }

    public Transform PlayerSpawn {
        get{
            return playerSpawn;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        vcam = transform.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        var grid = transform.Find("Grid");
        etherMap = grid.Find("EtherWorld").gameObject;
        normalMap = grid.Find("NormalWorld").gameObject;
        playerSpawn = transform.Find("PlayerSpawn");
    }

    public CinemachineVirtualCamera VCam {
        get{
            return vcam;
        }
    }
}
