using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    Rigidbody rgb;
    [SerializeField]
    int speed=1;

    void Start() {
        rgb=GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        rotate();
        move();
        exit();
    }

    void rotate() {
        Vector2 mousePos=Input.mousePosition;
        mousePos.x/=Screen.width;
        mousePos.y/=Screen.height;
        mousePos.x=mousePos.x*2-1;
        mousePos.y=mousePos.y*2-1;
        transform.localEulerAngles=new Vector3(-mousePos.y*90,mousePos.x*360,0);
    }

    void move() {
        float xinput=Input.GetAxis("Horizontal");
        float zinput=Input.GetAxis("Vertical");
        Vector3 dest=new Vector3(xinput,0,zinput).normalized;
        rgb.AddRelativeForce(dest*speed);
    }

    void exit() {
        if(Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
