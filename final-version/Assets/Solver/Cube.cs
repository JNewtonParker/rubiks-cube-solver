using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Solver{
    public class Cube
    {
        uint[] bitboards; //of length six, this stores a full representation of a cube (one index per face)
        uint[] sequence; //stores the sequence that has been applied to another cube to reach this one
        uint lastMove; //stores the move that was last applied to find this one (0 if sequence is empty)

        //CONSTRUCTORS

        public Cube()
        { //intialises an unrotated, solved cube

            //U,D,L,R,B,F
            this.bitboards = new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x33333333, 0x55555555, 0x99999999, 0x66666666};

            this.sequence = new uint[0];
            this.lastMove = 0;
        }

        public Cube(uint[] bitboards)
        {//initialises a new cube with the passed bitboard as state
           
            this.bitboards = new uint[bitboards.Length];
            Array.Copy(bitboards, this.bitboards, bitboards.Length);

            this.sequence = new uint[0];
            this.lastMove = 0;
        }

        private Cube(Cube oldCube, uint move)
        {//define a new cube as the result of rotating another

            this.bitboards = oldCube.RotatedBitboards(move);

            this.sequence = new uint[oldCube.sequence.Length+1];
            Array.Copy(oldCube.sequence, this.sequence, oldCube.sequence.Length);
            this.sequence[this.sequence.Length -1] = move;

            this.lastMove = move;
        }

        //COPYING
    
        public Cube CleanCopy()
        { //copies the state of a cube, without its history
            return new Cube(this.bitboards);
        }

        //RESULTS OF ROTATIONS


        public Cube CleanRotatedBySequence(uint[] sequence)
        {//returns a cube with the resultant bitboards, and no history

            Cube newCube = new Cube(this.bitboards);

            //for each move in the sequence the cube is being rotated by
            foreach (uint move in sequence){

                //rotate the cube
                newCube.bitboards = newCube.RotatedBitboards(move);
            }

            //return the new cube
            return newCube;
        }
        
        public Cube RotatedBySequence(uint[] sequence)
        {//returns the cube resulting from the sequence, with history

            //get a cube with the correct bitboards
            Cube newCube = CleanRotatedBySequence(sequence);

            //if the cube sequence needs to be changed
            if (sequence.Length > 0)
            {
                //copy the sequence into the cube
                newCube.sequence = new uint[this.sequence.Length + sequence.Length];
                Array.Copy(this.sequence, newCube.sequence, this.sequence.Length);

                for (int i = 0; i < sequence.Length; i++)
                {
                    newCube.sequence[this.sequence.Length+i] = sequence[i];
                }

                //and set the last move to be the last move applied
                newCube.lastMove = sequence[sequence.Length-1];
            }
            
            // return the new cube
            return newCube;
        }

        public Cube RotatedWithLastMove(uint move)
        { //returns the cube resulting from applying the passed move to a cube, without sequence but with last move

            //get a clean cube with rotated bitboards, and set the last move
            Cube cube = new Cube(RotatedBitboards(move));
            cube.lastMove = move;

            //return the cube
            return cube;
        }

        public Cube RotatedByMove(uint move)
        {//returns the cube that would result from applying the passed move to the current cube
           
            return new Cube(this, move);
        }
       
         //ROTATIONS

        private uint[] RotatedBitboards(uint move)
        { //returns the bitboards resulting by turing the curent object by the move passed

            //switch by move. the moves are consistent with those defined in initialMoves.
            switch (move) { 
                case 1:
                    return RotateU1();
                case 7:
                    return RotateU2();
                case 13:
                    return RotateU3();
                case 2:
                    return RotateD1();
                case 8:
                    return RotateD2();
                case 14:
                    return RotateD3();
                case 3:
                    return RotateL1();
                case 9:
                    return RotateL2();
                case 15:
                    return RotateL3();
                case 4:
                    return RotateR1();
                case 10:
                    return RotateR2();
                case 16:
                    return RotateR3();
                case 5:
                    return RotateB1();
                case 11:
                    return RotateB2();
                case 17:
                    return RotateB3();
                case 6:
                    return RotateF1();
                case 12:
                    return RotateF2();
                case 18:
                    return RotateF3();

                //if the passed move isn't in the list, return a new array storing bitboards matching this one
                default:
                    uint[] bitboards = new uint[this.bitboards.Length];
                    Array.Copy(this.bitboards, bitboards, this.bitboards.Length);
                    return bitboards;
            }

        }

        private uint[] RotateU1()
        { //return the bitboards of this cube with the U face rotated clockwise once

            //masks all but first three edges. For L,R,F,B faces, these are the pieces not attached to the U face.
            uint mask = 0xFFFFF; 

            uint[] rotated = new uint[6];

             //rotate U-Face
            rotated[0] = RotateBitboard(this.bitboards[0], 8);

            //take bits un-rotated from original face, and the bits being rotated to this face from where they were before.
            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[2])); 
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[4]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[3]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[5]));

            //opposite side stays the same
            rotated[1] = this.bitboards[1]; 

            //return the resulting bitboards
            return rotated;

        }

        private uint[] RotateU2()
        { //return the bitboards of this cube with the U face rotated clockwise twice
            
            uint mask = 0xFFFFF;
            uint[] rotated = new uint[6];

            rotated[0] = RotateBitboard(this.bitboards[0], 16);

            //same as U1 but shifted one further
            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[5])); 
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[2]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[4]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[3]));

            rotated[1] = this.bitboards[1];

            return rotated;

        }

        private uint[] RotateU3()
        { //return the bitboards of this cube with the U face rotated clockwise three times

            uint mask = 0xFFFFF; //ony one needed
            uint[] rotated = new uint[6];

            rotated[0] = RotateBitboard(this.bitboards[0], 24);

            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[3]));
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[5]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[2]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[4]));

            rotated[1] = this.bitboards[1];

            return rotated;

            //note this is effectively calling U1 three times, or doing it in reverse
        }

        private uint[] RotateD1()
        {  //return the bitboards of this cube with the D face rotated clockwise once

            uint mask = 0xFFFF000F; //Mask takes the three pieces of a face on the 'bottom'. For L,R,F,B, these are the edges also on the D Face.

            uint[] rotated = new uint[6];

            rotated[1] = RotateBitboard(this.bitboards[1], 8);

            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[3]));
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[5]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[2]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[4]));

            rotated[0] = this.bitboards[0];

            return rotated;

        }

        private uint[] RotateD2()
        {  //return the bitboards of this cube with the D face rotated clockwise twice

            uint mask = 0xFFFF000F;

            uint[] rotated = new uint[6];

            rotated[1] = RotateBitboard(this.bitboards[1], 16);

            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[5]));
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[2]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[4]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[3]));

            rotated[0] = this.bitboards[0];

            return rotated;
        }

        private uint[] RotateD3()
        { //return the bitboards of this cube with the D face rotated clockwise three times

            uint mask = 0xFFFF000F;

            uint[] rotated = new uint[6];

            rotated[1] = RotateBitboard(this.bitboards[1], 24);

            rotated[4] = ((mask & this.bitboards[4]) | (~mask & this.bitboards[2]));
            rotated[3] = ((mask & this.bitboards[3]) | (~mask & this.bitboards[4]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[3]));
            rotated[2] = ((mask & this.bitboards[2]) | (~mask & this.bitboards[5]));

            rotated[0] = this.bitboards[0];

            return rotated;

        }

        private uint[] RotateL1()
        { //return the bitboards of this cube with the L face rotated clockwise once

            //takes the three pieces on the left (first and last two stored, 0x0FFFFF00)
            uint mask = 0xFFFFF00; 

            uint[] rotated = new uint[6];

            //back face is rotated in the same way as U, F, D, but has a different set of pieces in required place. Shift to match.
            uint B = RotateBitboard(this.bitboards[4], 16); 

            rotated[2] = (RotateBitboard(this.bitboards[2], 8));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & B));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[1]), 16)); //B, then rotated back
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & this.bitboards[5]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[0]));

            rotated[3] = this.bitboards[3];

            return rotated;

        }

        private uint[] RotateL2()
        { //return the bitboards of this cube with the L face rotated clockwise twice

            uint mask = 0xFFFFF00;

            uint[] rotated = new uint[6];

            uint B = RotateBitboard(this.bitboards[4], 16);

            rotated[2] = (RotateBitboard(this.bitboards[2], 16));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & this.bitboards[1]));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[5]), 16));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & this.bitboards[0]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & B));

            rotated[3] = this.bitboards[3];

            return rotated;

        }

        private uint[] RotateL3()
        { //return the bitboards of this cube with the L face rotated clockwise three times

            uint mask = 0xFFFFF00;

            uint[] rotated = new uint[6];

            uint B = RotateBitboard(this.bitboards[4], 16);

            rotated[2] = RotateBitboard(this.bitboards[2], 24);

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & this.bitboards[5]));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[0]), 16));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & B));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[1]));

            rotated[3] = this.bitboards[3];

            return rotated;

        }

        private uint[] RotateR1()
        { //return the bitboards of this cube with the R face rotated clockwise once

            //get the three right pieces
            uint mask = 0xFF000FFF;

            uint[] rotated = new uint[6];

            uint B = RotateBitboard(this.bitboards[4], 16);

            rotated[3] = (RotateBitboard(this.bitboards[3], 8));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & this.bitboards[5]));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[0]), 16));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & B));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[1]));

            rotated[2] = this.bitboards[2];

            return rotated;

        }

        private uint[] RotateR2()
        { //return the bitboards of this cube with the R face rotated clockwise twice
             
            uint mask = 0xFF000FFF;

            uint[] rotated = new uint[6];

            uint B = RotateBitboard(this.bitboards[4], 16);

            rotated[3] = (RotateBitboard(this.bitboards[3], 16));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & this.bitboards[1]));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[5]), 16));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & this.bitboards[0]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & B));

            rotated[2] = this.bitboards[2];

            return rotated;

        }

        private uint[] RotateR3()
        { //return the bitboards of this cube with the R face rotated clockwise three times

            uint mask = 0xFF000FFF;

            uint[] rotated = new uint[6];

            uint B = RotateBitboard(this.bitboards[4], 16);

            rotated[3] = (RotateBitboard(this.bitboards[3], 24));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & B));
            rotated[4] = (RotateBitboard((mask & B) | (~mask & this.bitboards[1]), 16));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & this.bitboards[5]));
            rotated[5] = ((mask & this.bitboards[5]) | (~mask & this.bitboards[0]));

            rotated[2] = this.bitboards[2];

            return rotated;

        }

        private uint[] RotateB1()
        { //return the bitboards of this cube with the B face rotated clockwise once

            /*
            initially, the relevant bits are...

            yellow 00011111
            orange 11000111
            white 11110001
            red 01111100

            ...where the 1's are. Shift R, L, D to be in the same position as U, then these can be uinterchanged freely.
            */

            //final 5 pieces. ~mask becomes first three pieces, or the ones on the 'top'
            uint mask = 0xFFFFF; 

            //R,L,D faces are rotated incorrectly to be interacted with via the mask. shift to match.
            uint R = RotateBitboard(this.bitboards[3], 24);
            uint L = RotateBitboard(this.bitboards[2], 8);
            uint D = RotateBitboard(this.bitboards[1], 16);

            uint[] rotated = new uint[6];

            rotated[4] = (RotateBitboard(this.bitboards[4], 8));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & R));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & D), 8));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & this.bitboards[0]), 24));
            rotated[1] = (RotateBitboard((mask & D) | (~mask & L), 16));

            rotated[5] = this.bitboards[5];

            return rotated;

        }

        private uint[] RotateB2()
        { //return the bitboards of this cube with the B face rotated clockwise twice

            uint mask = 0xFFFFF;

            uint R = RotateBitboard(this.bitboards[3], 24);
            uint L = RotateBitboard(this.bitboards[2], 8);
            uint D = RotateBitboard(this.bitboards[1], 16);

            uint[] rotated = new uint[6];

            rotated[4] = (RotateBitboard(this.bitboards[4], 16));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & D));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & L), 8));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & R), 24));
            rotated[1] = (RotateBitboard((mask & D) | (~mask & this.bitboards[0]), 16));

            rotated[5] = this.bitboards[5];

            return rotated;

        }

        private uint[] RotateB3()
        { //return the bitboards of this cube with the B face rotated clockwise three times

            uint mask = 0xFFFFF;

            uint R = RotateBitboard(this.bitboards[3], 24);
            uint L = RotateBitboard(this.bitboards[2], 8);
            uint D = RotateBitboard(this.bitboards[1], 16);

            uint[] rotated = new uint[6];

            rotated[4] = (RotateBitboard(this.bitboards[4], 24));

            rotated[0] = ((mask & this.bitboards[0]) | (~mask & L));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & this.bitboards[0]), 8));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & D), 24));
            rotated[1] = (RotateBitboard((mask & D) | (~mask & R), 16));

            rotated[5] = this.bitboards[5];

            return rotated;

        }

        private uint[] RotateF1()
        { //return the bitboards of this cube with the F face rotated clockwise once

            /*
            initially, the relevant bits are...

            yellow 11110001
            orange 01111100
            white 00011111
            red 11000111

         ...where the 1's are. Shift R, L, D to be in the same position as U, then these can be uinterchanged freely.
            */

            uint mask = 0xFFFFF;

            uint U = RotateBitboard(this.bitboards[0], 16);
            uint R = RotateBitboard(this.bitboards[3], 8);
            uint L = RotateBitboard(this.bitboards[2], 24);

            uint[] rotated = new uint[6];

            rotated[5] = (RotateBitboard(this.bitboards[5], 8));

            rotated[0] = (RotateBitboard((mask & U) | (~mask & L), 16));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & U), 24));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & this.bitboards[1]), 8));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & R));

            rotated[4] = this.bitboards[4];

            return rotated;

        }

        private uint[] RotateF2()
        { //return the bitboards of this cube with the F face rotated clockwise twice

            uint mask = 0xFFFFF;

            uint U = RotateBitboard(this.bitboards[0], 16);
            uint R = RotateBitboard(this.bitboards[3], 8);
            uint L = RotateBitboard(this.bitboards[2], 24);

            uint[] rotated = new uint[6];

            rotated[5] = (RotateBitboard(this.bitboards[5], 16));

            rotated[0] = (RotateBitboard((mask & U) | (~mask & this.bitboards[1]), 16));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & L), 24));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & R), 8));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & U));

            rotated[4] = this.bitboards[4];

            return rotated;

        }

        private uint[] RotateF3()
        { //return the bitboards of this cube with the F face rotated clockwise three times

            uint mask = 0xFFFFF;

            uint U = RotateBitboard(this.bitboards[0], 16);
            uint R = RotateBitboard(this.bitboards[3], 8);
            uint L = RotateBitboard(this.bitboards[2], 24);

            uint[] rotated = new uint[6];

            rotated[5] = (RotateBitboard(this.bitboards[5], 24));

            rotated[0] = (RotateBitboard((mask & U) | (~mask & R), 16));
            rotated[3] = (RotateBitboard((mask & R) | (~mask & this.bitboards[1]), 24));
            rotated[2] = (RotateBitboard((mask & L) | (~mask & U), 8));
            rotated[1] = ((mask & this.bitboards[1]) | (~mask & L));

            rotated[4] = this.bitboards[4];

            return rotated;

        }

        //INFO RETRIVAL

        public uint GetRelatedEdge(uint face, int shift)
        { //returns the edge relating the face and shift. i.e. the other side of the piece found at the passed location

            uint lastPieceMask = 0xF;

            switch(face)
            {
                //case 0 would be all the U edges
                case 0:
                    switch (shift)
                    {

                        //case 0, 0 is UL edge from U. This is L face (2),  and needs to be shifted 3 positions (8*3 = 24) to be in its own last position.
                        case 0: 
                            return this.bitboards[2] >> 24 & lastPieceMask;
                        case 8:
                            return this.bitboards[5] >> 24 & lastPieceMask;
                        case 16:
                            return this.bitboards[3] >> 24 & lastPieceMask;
                        case 24:
                            return this.bitboards[4] >> 24 & lastPieceMask;
                    }

                    //needed to compile
                    break;

                case 1:
                    switch (shift)
                    {
                        case 0:
                            return this.bitboards[2] >> 8 & lastPieceMask;
                        case 8:
                            return this.bitboards[4] >> 8 & lastPieceMask;
                        case 16:
                            return this.bitboards[3] >> 8 & lastPieceMask;
                        case 24:
                            return this.bitboards[5] >> 8 & lastPieceMask;
                    }
                    break;
                case 2:
                    switch (shift)
                    {
                        case 0:
                            return this.bitboards[4] >> 16 & lastPieceMask;
                        case 8:
                            return this.bitboards[1] & lastPieceMask;
                        case 16:
                            return this.bitboards[5] & lastPieceMask;
                        case 24:
                            return this.bitboards[0] & lastPieceMask;
                    }
                    break;
                case 3:
                    switch (shift)
                    {
                        case 0:
                            return this.bitboards[5] >> 16 & lastPieceMask;
                        case 8:
                            return this.bitboards[1] >> 16 & lastPieceMask;
                        case 16:
                            return this.bitboards[4] & lastPieceMask;
                        case 24:
                            return this.bitboards[0] >> 16 & lastPieceMask;
                    }
                    break;
                case 4:
                    switch (shift)
                    {
                        case 0:
                            return this.bitboards[3] >> 16 & lastPieceMask;
                        case 8:
                            return this.bitboards[1] >> 8 & lastPieceMask;
                        case 16:
                            return this.bitboards[2] & lastPieceMask;
                        case 24:
                            return this.bitboards[0] >> 24 & lastPieceMask;
                    }
                    break;
                case 5:
                    switch (shift)
                    {
                        case 0:
                            return this.bitboards[2] >> 16 & lastPieceMask;
                        case 8:
                            return this.bitboards[1] >> 24 & lastPieceMask;
                        case 16:
                            return this.bitboards[3] & lastPieceMask;
                        case 24:
                            return this.bitboards[0] >> 8 & lastPieceMask;
                    }  
                    break;  
            }

            //if passed face/shift are invalid, return 0. i.e. no piece
            return 0;
        }

        public uint GetClockwiseRelatedCorner(uint face, int shift)
        { ////returns the final face of the corner relating the face and shift. i.e. the next side of the piece found at the passed location
            uint lastPieceMask = 0xF;

            switch (face)
            {
                case 0:
                    switch (shift)
                    {

                        //shifts here are all +4, as the corners are stored intertwined with the edges, edge-corner-edge-corner, etc. Each sticker is 4 bits, so an extra four needed.
                        case 4: 
                            return this.bitboards[5] >> 28 & lastPieceMask;
                        case 12:
                            return this.bitboards[3] >> 28 & lastPieceMask;
                        case 20:
                            return this.bitboards[4] >> 28 & lastPieceMask;
                        case 28:
                            return this.bitboards[2] >> 28 & lastPieceMask;
                    }
                    break;

                case 1:
                    switch (shift)
                    {
                        case 4:
                            return this.bitboards[4] >> 12 & lastPieceMask;
                        case 12:
                            return this.bitboards[3] >> 12 & lastPieceMask;
                        case 20:
                            return this.bitboards[5] >> 12 & lastPieceMask;
                        case 28:
                            return this.bitboards[2] >> 12 & lastPieceMask;
                    }
                    break;
                case 2:
                    switch (shift)
                    {
                        case 4:
                            return this.bitboards[1] >> 4 & lastPieceMask;
                        case 12:
                            return this.bitboards[5] >> 4 & lastPieceMask;
                        case 20:
                            return this.bitboards[0] >> 4 & lastPieceMask;
                        case 28:
                            return this.bitboards[4] >> 20 & lastPieceMask;
                    }
                    break;
                case 3:
                    switch (shift)
                    {
                        case 4:
                            return this.bitboards[1] >> 20 & lastPieceMask;
                        case 12:
                            return this.bitboards[4] >> 4 & lastPieceMask;
                        case 20:
                            return this.bitboards[0] >> 20 & lastPieceMask;
                        case 28:
                            return this.bitboards[5] >> 20 & lastPieceMask;
                    }
                    break;
                case 4:
                    switch (shift)
                    {
                        case 4:
                            return this.bitboards[1] >> 12 & lastPieceMask;
                        case 12:
                            return this.bitboards[2] >> 4 & lastPieceMask;
                        case 20:
                            return this.bitboards[0] >> 28 & lastPieceMask;
                        case 28:
                            return this.bitboards[3] >> 20 & lastPieceMask;
                    }
                    break;
                case 5:
                    switch (shift)
                    {
                        case 4:
                            return this.bitboards[1] >> 28 & lastPieceMask;
                        case 12:
                            return this.bitboards[3] >> 4 & lastPieceMask;
                        case 20:
                            return this.bitboards[0] >> 12 & lastPieceMask;
                        case 28:
                            return this.bitboards[2] >> 20 & lastPieceMask;
                    }
                    break;
            }

            //if face and shift were invalid, return a piece with no info
            return 0;
        }

        public uint GetCornerPosition (uint face, int shift)
        { //returns a value representing the position of a corner. this representation is internally consistent, but arbitrary otherwise

            //face + shift will be unique for all combinations, as the gap between each shift is bigger than the highest value for face.
            switch(face+shift){
                case 28: //Ybr
                case 4+20: //yBr
                case 2+28: //ybR
                    return 1;
                case 20: //Ybo
                case 4+28: //yBo
                case 3+20: //ybO
                    return 2;
                case 12: //Ygo
                case 5+20: //yGo
                case 3+28: //ygO
                    return 3;
                case 4: //Ygr
                case 5+28: //yGr
                case 2+20: //ygR
                    return 4;
                case 1+4: //Wbr
                case 4+12: //wBr
                case 2+4: //wbR
                    return 5;
                case 1+12: //Wbo
                case 4+4: //wBo
                case 3+12: //wbO
                    return 6;
                case 1+20: //Wgo
                case 5+12: //wGo
                case 3+4: //wgO
                    return 7;
                case 1+28: //Wgr
                case 5+4: //wGr
                case 2+12: //wgR
                    return 8;
            }

            //return 0 if face + shift were invalid
            return 0;
        }

        //TWO GEN STATE INFO

        public bool IsTwoGenIfLeftBlock()
        { //returns true if the cube is reachable from G_2 provided it has a left block already established


            uint lastPieceMask = 0xF;

            //find BFR and put in slot

            //get a cube with corners permuted such that the two corners that go on the D Face are there (though not necessarly oriented).
            Cube twoGenCube = PermutedDCornersOutcome(); 

            //cube is now in a state such that the four U corners are on the U face, correctly permuted. Corners will go U -> R -> W; Up, Right, Wrong, rotated clockwise.

            //only the top face is relevant for the check once above functions are complete. Start empty, add relevant information.
            uint UBitboard = 0;

            //for each corner, the intention is to add the colour found by rotating from U once.

            for (int shift = 4; shift<=28; shift+=8)
            { 
                //get a mask that retrieves only the required corner
                uint cornerMask = lastPieceMask << shift;

                //get the next corner, and the corner relating to it
                uint corner = (twoGenCube.bitboards[0] >> shift) & lastPieceMask;
                uint relatedCorner = twoGenCube.GetClockwiseRelatedCorner(0, shift);

                //if the corner is U, add the relatedCorner shifted back to where it was originally
                if (corner == 12){
                    UBitboard = UBitboard | (relatedCorner << shift); 

                }
                // else if this corner is not U (R/W), if clockwise related corner is also not U, this is R. (R -> W, W -> U.) If X -!> Y, X = R).
                else if (relatedCorner != 12)
                {
                    //add the corner shifted back to where it was originally
                    UBitboard = UBitboard | (corner << shift);
                }

                //relatedCorner = U implies corner is the wrong (third) colour. Explicit conversions done for this case.
                else
                { 
                    switch (corner)
                    {
                        // for case 9, wrong is B Face, i.e. the UBR corner. needs to add representation for L.
                        case 9:
                            UBitboard = UBitboard | (0x33333333 & cornerMask); //B -> Y -> R
                            break;
                        case 5:
                            UBitboard = UBitboard | (0x99999999 & cornerMask); //O -> Y -> B
                            break;
                        case 6:
                            UBitboard = UBitboard | (0x55555555 & cornerMask); //G -> Y -> O
                            break;
                        case 3:
                            UBitboard = UBitboard | (0x66666666 & cornerMask); //R -> Y -> G
                            break;
                    }
                }
            }

            //cube is now in a state such that the four U corners are the colours of their clockwise relations. It should be in the cycle R -> G -> O -> B

            //representation of clockwise R -> G -> O -> B. values are checked in a cycle, and could be stored in any related order (i.e. 5->6->3->9)
            uint desired = 0x30905060;

            //rotate the found board by 8 more bits each time. 4 rotations needed to check all states (current, +1, +2, +3)
            for (int i = 0; i< 4; i++) { 
                
                //if the bitboard rotated by the shfit * 8 is equal to the desired cube, then it is not reachable by G_2 and the function can return true
                if (RotateBitboard(UBitboard, i*8) == desired) return true;
            }

            //otherwise the cube is not reachable from G_2
            return false;
        }

        private Cube PermutedDCornersOutcome()
        {//returns a cube with corners permuted such that the two corners that go on the D Face are there (though not necessarly oriented).

            //copy the current cube (sequence doens't matter and CleanCopy is faster)
            Cube twoGenCube = this.CleanCopy(); 

            uint lastPieceMask = 0xF;

            //put DFR piece in back down right position, orientation doesn't matter.
            
            for (uint face = 0; face < 6; face++)
            {
                for (int shift = 4; shift <= 28; shift += 8)
                {
                    //get next corner
                    uint corner = (twoGenCube.bitboards[face] >> shift) & lastPieceMask; // no need for + 4 on this corner as is in loop

                    //if corner is from the D face and the clockwise related corner is from F
                    if (corner == 10 && twoGenCube.GetClockwiseRelatedCorner(face, shift) == 6) 
                    {
                        //get the position of the corner on the cube
                        uint position = twoGenCube.GetCornerPosition(face, shift);

                        //rotate the appropriately such that DFR piece is in the location DBR
                        switch (position)
                        {
                            //if top back left, insert as follows.
                            case 1: 
                                twoGenCube.bitboards = twoGenCube.RotateU1();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 2: //top back right
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 3: //top front right
                                twoGenCube.bitboards = twoGenCube.RotateR2();
                                break;
                            case 4: //top front left
                                twoGenCube.bitboards = twoGenCube.RotateU2();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 7: //bottom front right
                                twoGenCube.bitboards = twoGenCube.RotateR3();
                                break;
                        }
                        //note switch statement skips 5, 6, 7, 8. 5, 8 are bottom left, and are known to be solved by this stage already. 6 was solved in insertBWO. 7 is in position.
                    }
                }
            }

            //put DBR in back down right spot, and shift the current back down right piece (DFR) into front down right position.

            for (uint face = 0; face < 6; face++)
            {
                for (int shift = 4; shift <= 28; shift += 8)
                {
                
                    uint corner = (twoGenCube.bitboards[face] >> shift) & lastPieceMask;

                    //if corner is D and clockwise related coner is R
                    if (corner == 10 && twoGenCube.GetClockwiseRelatedCorner(face, shift) == 5)
                    {
                    
                        //get the position of this corner on the cube
                        uint position = twoGenCube.GetCornerPosition(face, shift); 

                        //insert it, while also shifting the pice currently in DBR to DFR
                        switch (position)
                        {
                            case 1: //if top back left, insert as follows.
                                twoGenCube.bitboards = twoGenCube.RotateU1();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 2: //top back right
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 3: //top front right
                                twoGenCube.bitboards = twoGenCube.RotateU3();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 4: //top front left
                                twoGenCube.bitboards = twoGenCube.RotateU2();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                break;
                            case 7: //bottom front right
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                twoGenCube.bitboards = twoGenCube.RotateU1();
                                twoGenCube.bitboards = twoGenCube.RotateR1();
                                twoGenCube.bitboards = twoGenCube.RotateU3();
                                twoGenCube.bitboards = twoGenCube.RotateR2();
                                break;
                        } //note switch statement skips 5, 6, 8. 5, 8 are bottom left, and are known to be solved by this stage already. 6 is in position.
                    }
                }
            }

            //return the rotated cube
            return twoGenCube;
        }

        //GETTERS

        public uint[] GetBitboards()
        { //getter for bitboards

            return this.bitboards;
        }

        public uint[] GetSequence()
        { //getter for sequence

            return sequence;
        }

        public uint GetLastMove()
        {//getter for lastMove

            return lastMove;
        }

        //UTILITY

        public override bool Equals(object o)
        { //returns true if the passed object is a cube (No error prevention) and the bitboards of both cubes match. sequence has no effect.

             return Enumerable.SequenceEqual(this.bitboards, ((Cube) o).bitboards);
        }

        public override int GetHashCode()
        {//function not implemented

            throw new Exception("No HashCode Implemented");
        }

        public override string ToString()
        { //returns a string equivalent for the cube, layed out as a map

            //defined a map to be used to convert from hex representation to standard cube colours.
            Dictionary<char, string> colourMap = new Dictionary<char, string>();

            colourMap['c'] = "Y";
            colourMap['9'] = "B";
            colourMap['5'] = "O";
            colourMap['6'] = "G";
            colourMap['3'] = "R";
            colourMap['a'] = "W";
            colourMap['0'] = "-"; //if a piece is missing, use '-'
            colourMap['f'] = "?"; //if a piece is set to be anything, use '?' (this is used for 4-gen)

            //convert bitboards to hex string
            string[] hexStringCube = new string[]{
                    this.bitboards[0].ToString("X"),
                    this.bitboards[4].ToString("X"),
                    this.bitboards[3].ToString("X"),
                    this.bitboards[5].ToString("X"),
                    this.bitboards[2].ToString("X"),
                    this.bitboards[1].ToString("X")
                };

            //padd each hexString with 0s so they're of length 8
            for (int i = 0; i < hexStringCube.Length; i++){
                if (hexStringCube[i].Length < 8){
                    hexStringCube[i] = new string('0', 8-hexStringCube[i].Length) + hexStringCube[i];
                }
            }

            //produce map and return it if no errors, otherwise return the empty string
            try {
                return
                        // YELLOW
                        "\t" +
                                colourMap[hexStringCube[0][0]] +
                                colourMap[hexStringCube[0][1]] +
                                colourMap[hexStringCube[0][2]] +
                                "\n" +
                                "\t" +
                                colourMap[hexStringCube[0][7]] +
                                "Y" +
                                colourMap[hexStringCube[0][3]] +
                                "\n" +
                                "\t" +
                                colourMap[hexStringCube[0][6]] +
                                colourMap[hexStringCube[0][5]] +
                                colourMap[hexStringCube[0][4]] +
                                "\n" +

                                //MIDDLE TOP
                                colourMap[hexStringCube[4][0]] +
                                colourMap[hexStringCube[4][1]] +
                                colourMap[hexStringCube[4][2]] +
                                " " +
                                colourMap[hexStringCube[3][0]] +
                                colourMap[hexStringCube[3][1]] +
                                colourMap[hexStringCube[3][2]] +
                                " " +
                                colourMap[hexStringCube[2][0]] +
                                colourMap[hexStringCube[2][1]] +
                                colourMap[hexStringCube[2][2]] +
                                " " +
                                colourMap[hexStringCube[1][0]] +
                                colourMap[hexStringCube[1][1]] +
                                colourMap[hexStringCube[1][2]] +
                                "\n" +

                                // MIDDLE
                                colourMap[hexStringCube[4][7]] +
                                "R" +
                                colourMap[hexStringCube[4][3]] +
                                " " +
                                colourMap[hexStringCube[3][7]] +
                                "G" +
                                colourMap[hexStringCube[3][3]] +
                                " " +
                                colourMap[hexStringCube[2][7]] +
                                "O" +
                                colourMap[hexStringCube[2][3]] +
                                " " +
                                colourMap[hexStringCube[1][7]] +
                                "B" +
                                colourMap[hexStringCube[1][3]] +
                                "\n" +

                                // MIDDLE BOTTOM
                                colourMap[hexStringCube[4][6]] +
                                colourMap[hexStringCube[4][5]] +
                                colourMap[hexStringCube[4][4]] +
                                " " +
                                colourMap[hexStringCube[3][6]] +
                                colourMap[hexStringCube[3][5]] +
                                colourMap[hexStringCube[3][4]] +
                                " " +
                                colourMap[hexStringCube[2][6]] +
                                colourMap[hexStringCube[2][5]] +
                                colourMap[hexStringCube[2][4]] +
                                " " +
                                colourMap[hexStringCube[1][6]] +
                                colourMap[hexStringCube[1][5]] +
                                colourMap[hexStringCube[1][4]] +
                                "\n" +

                                // WHITE
                                "\t" +
                                colourMap[hexStringCube[5][0]] +
                                colourMap[hexStringCube[5][1]] +
                                colourMap[hexStringCube[5][2]] +
                                "\n" +
                                "\t" +
                                colourMap[hexStringCube[5][7]] +
                                "W" +
                                colourMap[hexStringCube[5][3]] +
                                "\n" +
                                "\t" +
                                colourMap[hexStringCube[5][6]] +
                                colourMap[hexStringCube[5][5]] +
                                colourMap[hexStringCube[5][4]] +
                                "\n";

            }
            catch
            {
                return "";
            }

        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //recommends compiler to inline method
        private uint RotateBitboard(uint bitboard, int shift)
        {//rotates a bitboard cyclically, i.e. 12345 shifted 2 is 45123

            return (bitboard >> shift) | (bitboard << (32 - shift));
        }
    }

}