using System.Collections;
using System.Collections.Generic;

using static System.Math;

using UnityEngine;

public class RotateCube : MonoBehaviour
{

    public GameObject target; //target rotation

    Vector2 initialMousePosition; //initial mouse position when cube is held
    bool holding = false; //flag for cube being held

    float speed = 250f; //speed of rotation

    void Update()
    { //called every frame

        //if right click was being held down, starting from over the cube
        if (holding){

            //if it has now been released
            if (Input.GetMouseButtonUp(0))
            { 
                    //cube is no longer being held and thwe target's rotation can be set to the desired final rotation
                    holding = false;
                    RotateTarget();

            } else {

                //continue dragging cube if still held down
                DragCube();
            }
        } else {

            //if not holding, adjust cube to sensible rotation
            AdjustCubeToMatchTarget();
        }
        
    }

    void OnMouseOver () 
    { //called each frame the mouse is hovering over the cube

        //if cube is left-clicked
        if (Input.GetMouseButtonDown(0))
        {

            //it is now being held. record initial position
            holding = true;
            initialMousePosition = Input.mousePosition;
        }
    }

    void DragCube()
    {//called each frame the mouse is being held down (starting over the cube)


        //get deltaMousePosition, the change in mouse position from the starting point (scaled)
        var deltaMousePosition = new Vector2(Input.mousePosition.x - initialMousePosition.x, Input.mousePosition.y - initialMousePosition.y);
        deltaMousePosition *= 0.005f;

        //rotate the cube towards this position
        transform.rotation = Quaternion.Euler(-deltaMousePosition.y, -deltaMousePosition.x, 0) * transform.rotation;
        
    }

    void RotateTarget()
    { //rotate the target to a reasonably oriented position for the cube to move towards

        //set target rotation to match the cube (could be maintained while turning the cube, but this is unnecessary)
        target.transform.rotation = transform.rotation;
        
        //get nearest correctly oriented rotation
        var vec = target.transform.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        target.transform.eulerAngles = vec;

        //get the change in mouse position from the starting point
        var deltaMousePosition = new Vector2(Input.mousePosition.x - initialMousePosition.x, Input.mousePosition.y - initialMousePosition.y);
        
        //if only a small movement, do one rotation. this avoids nothing being done for a small drag.
        if (Abs(deltaMousePosition.x) < 500 && Abs(deltaMousePosition.y) < 500)
        { 

            //if the turn is further along than it is up,it's probably a Y rotation
            if (Abs(deltaMousePosition.x) > Abs(deltaMousePosition.y))
            {
            
                //if the movement was from left to right
                if (deltaMousePosition.x > 0f)
                { 

                    //rotate the cube 90 degrees clockwise (to the left)
                    target.transform.Rotate(0,-90,0,Space.World);
                } 
                else 
                {
                    target.transform.Rotate(0,90,0,Space.World);
                }

            }
            //if this is a vertical movement, and if the movement is going up
            else if (deltaMousePosition.y > 0f)
            {

                //if it also went from left to right, it's probably a Z rotation
                if (deltaMousePosition.x > 0f)
                {
                    //rotate the cube up towards Z
                    target.transform.Rotate(0,0,90,Space.World);
                } 
                else
                { 

                    //else it was an X rotation. Rotate accordingly
                    target.transform.Rotate(-90,0,0,Space.World);
                }

            } 

            //else it must be a vertical, downwards movement
            else 
            { 
                //if it went from right to left, it's a Z rotation
                if (deltaMousePosition.x < 0f)
                {
                    target.transform.Rotate(0,0,-90,Space.World);
                } 
                else 
                {
                    target.transform.Rotate(90,0,0,Space.World);
                }
            }

        }
    }

     void AdjustCubeToMatchTarget()
    {//if necessary, each frame the cube is't being held, rotate the cube towards the target rotation

        //if they don't already match
        if (transform.rotation != target.transform.rotation)
        {
            //rotate the cube towards the target rotation.
            var step = speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
        }

        //note that this may not always be exactly correct due to floating point comparisons, but as it is in its own frame of reference it doesn't have any real effect
    }

  
}

