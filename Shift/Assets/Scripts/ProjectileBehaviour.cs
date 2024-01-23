using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileBehaviour : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public float speed;
    public bool direction;

    private Collider2D collider;
    private Tilemap currentMap;

    // Update is called once per frame
    void Update()
    {
        
        rigidbody2D.MovePosition(rigidbody2D.transform.position + Vector3.right * Time.deltaTime * speed);
        
    }

    void OnCollisionEnter2D(Collision2D col){
        Debug.Log(col.gameObject.name);
        if(col.gameObject.name == currentMap.name){
        
        ContactPoint2D[] lContacts = col.contacts;
        ContactPoint2D lPointContact = default;
        for(int i= 0; i < lContacts.Length; i++)
        {
            ///Check si c'est la bonne collision
            if(lContacts[i].collider.GetComponent<TilemapCollider2D>())
            {
                
                lPointContact = lContacts[i];
                break;
            }
        }
        
        
        Grid grid = currentMap.layoutGrid;
        Vector3Int cell = grid.WorldToCell(lPointContact.point + Vector2.right * 0.1f);
        Debug.Log(cell);
        
        currentMap.SetTile(cell,null);
        
        
        }
        Destroy(gameObject);
    }

    public void Init(Tilemap map){
        currentMap=map;
        Debug.Log(map.name);
    }
}
