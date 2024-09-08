using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSide : MonoBehaviour
{

    public List<GameObject> pieces; //all the pieces on the cube

    GameObject coreTarget; //the target rotation of the core (which pieces on a face are attached to)
    GameObject core; //attached to the pieces being rotated on a given side
 
    Queue<uint> moveQueue; //moves waiting to happen
    uint currentMove; //the move currenty being done

    float speed = 500f; //speed of rotation
    bool rotating = false; //if cube is currently rotating


    //USE CASES

    public void AddMoveToQueue(uint move) 
    { //used to add a single move to the queue

        //if move is a real move
        if (move >= 1 && move <= 18)
        {

            //add it to the queue
            moveQueue.Enqueue(move);
        }

    }

    public void AddMovesToQueue(uint[] moves)
    { //used to add a series of moves to a queue at the same time

        //for each move in the array
        foreach (uint move in moves)
        {
            //add it to the queue
            AddMoveToQueue(move);
        }

    }

    bool Finished()
    { //returns true if the queue is empty

        return (moveQueue.Count == 0);
    }

    //START

    void Start()
    { //called once at the start of the program

        //initialise the move queue
        moveQueue = new Queue<uint>();

        //the core is the first piece in the passed list
        core = pieces[0];
        pieces.RemoveAt(0);

        //the target is the second piece in the passed list (now pieces[0] as first has already been removed)
        coreTarget = pieces[0];
        pieces.RemoveAt(0);

    }
    
    //EACH FRAME

    void Update()
    {//called on every frame

        //if a rotation is not currently happening
        if (!rotating)
        {
            //if there are more moves to make
            if (moveQueue.Count > 0)
            {

                //start a new rotation
                StartRotation();
            }
        }

        //if the cube is currently rotating
        else
        {
            //continue rotating the cube
            ContinueRotation();
        }

    }

    void StartRotation()
    { //called at the start of every rotation

    
        //get the next move from the queue (also removes it from the queue)
        currentMove = moveQueue.Dequeue();

        //set the core's target transform to the transform the core currently has, with no floating point errors!
        var vec = core.transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        coreTarget.transform.localEulerAngles = vec;
        
        //for a given move
        switch (currentMove){

            //if U
            case 1:
                //the cube target rotation is 90 degrees clockwise
                coreTarget.transform.Rotate(0f,90f,0f,Space.Self);

                //attach U Face pieces to the core, such that when the core is rotated, so is this face.
                UFaceSetup();
                break;

            // if U2
            case 7:
                //same as above, but 180
                coreTarget.transform.Rotate(0f,180f,0f,Space.Self);

                UFaceSetup();
                break;
            
            //if U' or U3
            case 13:

                //same as above, but 270 (which is -90, there's no difference)
                coreTarget.transform.Rotate(0f,-90f,0f,Space.Self);

                UFaceSetup();
                break;

            case 2:
                coreTarget.transform.Rotate(0f,-90f,0f,Space.Self);
                DFaceSetup();
                break; 
            case 8:
                coreTarget.transform.Rotate(0f,180f,0f,Space.Self);
                DFaceSetup();
                break;
            case 14:
                coreTarget.transform.Rotate(0f,90f,0f,Space.Self);
                DFaceSetup();
                break;
            case 3:
                coreTarget.transform.Rotate(0f,0f,-90f,Space.Self);
                LFaceSetup();
                break;
            case 9:
                coreTarget.transform.Rotate(00f,0f,180f,Space.Self);
                LFaceSetup();
                break;
            case 15:
                coreTarget.transform.Rotate(0f,0f,90f,Space.Self);
                LFaceSetup();
                break;
            case 4:
                coreTarget.transform.Rotate(0f,0f,90f,Space.Self);
                RFaceSetup();
                break;
            case 10:
                coreTarget.transform.Rotate(0f,0f,180f,Space.Self);
                RFaceSetup();
                break;
            case 16:
                coreTarget.transform.Rotate(0f,0f,-90f,Space.Self);
                RFaceSetup();
                break;
            case 5:
                coreTarget.transform.Rotate(-90f,0f,0f,Space.Self);
                BFaceSetup();
                break;
            case 11:
                coreTarget.transform.Rotate(180f,0f,0f,Space.Self);
                BFaceSetup();
                break;
            case 17:
                coreTarget.transform.Rotate(90f,0f,0f,Space.Self);
                BFaceSetup();
                break;
            case 6:
                coreTarget.transform.Rotate(90f,0f,0f,Space.Self);
                FFaceSetup();
                break;
            case 12:
                coreTarget.transform.Rotate(180f,0f,0f,Space.Self);
                FFaceSetup();
                break;
            case 18:
                coreTarget.transform.Rotate(-90f,0f,0f,Space.Self);
                FFaceSetup();
                break;

            default:
                //if move was invalid, don't start a rotation. this will remove it from the queue without breaking the process, though this should never happen as move value is checked before it enters the queue
                return;
        }

        //set rotating to true, i.e. setup is complete and cube can now be rotated
        rotating = true;

    }

    void ContinueRotation()
    { //called each frame the cube is currently rotating
        
        //if the cube's orientation doesn't yet match the target
        if (core.transform.localRotation != coreTarget.transform.localRotation)
        {
            //rotate the core (and thus the pieces attached to the core) towards the target)
            var step = speed * Time.deltaTime;
            core.transform.localRotation = Quaternion.RotateTowards(core.transform.localRotation, coreTarget.transform.localRotation, step);
            
        //if the cubes orientation matches that of the target
        } else { 

            //end the rotation of this move
            EndRotation();
        }

        //note floating point errors could (and do) occur here, as the rotations equality will return true if they are nearly equal. this is appropriately handled in EndRotation()
    }

    void EndRotation()
    { //resets the parent hierarchy and avoids floating point errors at the end of each rotation
        
        //avoid the floating point errors by explicitly assigning the rotation of the core to that of the target. think of this as 'snapping' the face right at the end
        core.transform.localRotation = coreTarget.transform.localRotation;
        rotating = false;

        //for each piece on the cube
        foreach (GameObject piece in pieces)
        {

            //set the parent of each piece back to the original cube
            piece.transform.parent = transform;

            //and round each transform to the nearest whole number (this avoids the cube slowly falling apart)
            var vec = piece.transform.localPosition;
            vec.x = Mathf.Round(vec.x);
            vec.y = Mathf.Round(vec.y);
            vec.z = Mathf.Round(vec.z);
            piece.transform.localPosition = vec;

        }
 
        //reset core for next move (means the next rotation is true to the sides, not as a second rotation from the result of the previous)
        core.transform.localRotation = Quaternion.identity;
        core.transform.localPosition = Vector3.zero;

    }

    //ROTATION SETUP (core parent logic)

    void UFaceSetup()
    {//attaches the pieces on the U Face to the core (so they can be rotated)

        //for each piece in the cube
        foreach (GameObject piece in pieces)
        {
            //if it is on the U face
            if (piece.transform.localPosition.y == 1f)
            {

                //attach it to the core so that when the core is rotated, this piece is too
                piece.transform.parent = core.transform;
            }
        }

    }

    void DFaceSetup()
    {//attaches the pieces on the D Face to the core (so they can be rotated)

        foreach (GameObject piece in pieces)
        {
            //if it is on the D face
            if (piece.transform.localPosition.y == -1f)
            {
                piece.transform.parent = core.transform;
            }
        }

    }
    
    void LFaceSetup()
    {//attaches the pieces on the l Face to the core (so they can be rotated)

        foreach (GameObject piece in pieces)
        {
            //if it is on the L face
            if (piece.transform.localPosition.z == -1f)
            {
                piece.transform.parent = core.transform;
            }
        }

    }
    
    void RFaceSetup()
    {//attaches the pieces on the R Face to the core (so they can be rotated)

        foreach (GameObject piece in pieces)
        {
            //if it is on the R face
            if (piece.transform.localPosition.z == 1f)
            {
                piece.transform.parent = core.transform;
            }
        }

    }
    
    void BFaceSetup()
    {//attaches the pieces on the B Face to the core (so they can be rotated)

        foreach (GameObject piece in pieces)
        {
            //if it is on the B face
            if (piece.transform.localPosition.x == -1f)
            {
                piece.transform.parent = core.transform;
            }
        }

    }
    
    void FFaceSetup()
    {//attaches the pieces on the F Face to the core (so they can be rotated)

        foreach (GameObject piece in pieces)
        {
            //if it is on the F face
            if (piece.transform.localPosition.x == 1f)
            {
                piece.transform.parent = core.transform;
            }
        }

    }   
  
}