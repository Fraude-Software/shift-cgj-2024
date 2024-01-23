using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMecanics : MonoBehaviour
{

    private bool inputFire;

    private bool direction;
    public GameObject projectile;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Tilemap currentMap;


    // Start is called before the first frame update
    void Start()
    {
    
    }


    public void OnFire(InputAction.CallbackContext context){
        inputFire = context.ReadValueAsButton();
        if(context.started){
            Fire();
        }
    }

    // Update is called once per frame
    void Fire()
    {
        if(inputFire){
            Vector3 vector =default;
            if(spriteRenderer.flipX){
                vector = transform.position + Vector3.left;
                
                direction = true;
            }else{
                vector = transform.position + Vector3.right;
               
                direction = false;
                
            }

            GameObject proj = Instantiate(projectile, vector, Quaternion.identity);
            ProjectileBehaviour projBehavior = proj.GetComponent<ProjectileBehaviour>();
            projBehavior.direction = direction;
            projBehavior.Init(currentMap);
        }
       
    }
}
