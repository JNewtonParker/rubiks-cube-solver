using System;
using System.Collections.Generic;

namespace Solver{
    public class CubeSolver{

        //FOUR GEN
        
        public uint[] GetFourGenSolution(Cube cube)
        { //returns a sequence to convert a cube reachable by the generating set G_5 = {U,L,R,F,D} to one reachable by G_4 = {U,L,R,D}

            //define target as a cube representing all cubes generateed by the set G_4
            Cube target = new Cube(new uint[]{0xF0F0F0F, 0xF0F0F0F, 0, 0, 0xF000F, 0xF000F});

            //convert the passed cube to one with only information relevant for this reduction
            Cube start = GetFourGenMaskedCube(cube);

            //return an empty sequence if the passed cube is already reachable by G_4
            if (start.Equals(target)) return new uint[0];

            //define the list of moves that could end the conversion from G_5 to G_4
            uint[] targetMoves = new uint[]{6,18}; //F, F'. F2 Will maintain state

            //create a CubeStore storing the cubes reachable from the generic target cube via the moves that end the convertion
            CubeStore targetCubes = GetInitialTargets(target, targetMoves);

            //if start was one move away, get a sequence of the inverse of whatever the last move made was (as comes from the end)
            if (targetCubes.Contains(start)){
                 return new uint[]{GetMoveInverse(targetCubes.GetSequenceOfEqualCube(start)[0])};
            }

            //get the list of moves that can be used for this reduction (moves representing the generating set G_4)
            uint[] moves = new uint[]{1,2,3,4,6,7,8,9,10,12,13,14,15,16,18}; //a value % 6 is the same face. the incremental order is U,D,L,R,B,F

            //return the sequence that connects the masked passed cube (start) to those in the CubeStore
            return GetPathFromCubeToList(start, targetCubes, GetSubsequentMoves(moves));

            //note that only the moves {1,2,3,4,6} are needed, as 1+1+1 is equivalent to 13 (and 1+1 equivalent to 7), but that doing it in this way is computationally more efficient
        } 
        
        public Cube GetFourGenMaskedCube(Cube cube)
        { //returns a mask of the passed cube containing only values relevant for identifying if a cube can be reached by G_4

            //define start as empty initially
            uint[] start = new uint[]{0,0,0,0,0,0};

            //define a mask to strip a bitboard to its final piece. this is equivalent to 0x0000000F
            uint lastPieceMask = 0xF; 

            //for each face
            for (uint face = 0; face < 6; face++) 
            { 
                //for each edge shift (bitboard >> shift loops all edges)
                for (int shift = 0; shift <= 24; shift += 8) 
                { 
                    //get the next edge of the face
                    uint edge = (cube.GetBitboards()[face] >> shift) & lastPieceMask;

                    //if it is U or D, or is F or B and the related edge is not U or D (i.e FL, FL, BR)
                    if ((edge >= 10) | (edge >= 6 && cube.GetRelatedEdge(face, shift) < 9)) 
                    { 
                        //add this piece to the startingBitboard for the relevant face
                        start[face] = start[face] | (lastPieceMask << shift);
                    }
                }
            }

            //return the G_4 masked equivalent of the passed cube
            return new Cube(start);
        } 

        //THREE GEN

        public uint[] GetThreeGenSolution(Cube cube)
        { //returns a sequence to convert a cube reachable by the generating set G_5 = {U,L,R,D} to one reachable by G_4 = {U,L,R}

            //define target as a cube representing all cubes generateed by the set G_3. this is EO Line, but with only the B and F sticker stored.
            Cube target = new Cube(new uint[]{0, 0, 0, 0, 0x900, 0x600});

            //same as the masking for GetFourGenMaskedCube, but for the values relevant for G_3
            Cube start = GetThreeGenMaskedCube(cube);

            if (start.Equals(target)) return new uint[0];

            uint[] targetMoves = new uint[]{2,8,14}; //D Face
            CubeStore targetCubes = GetInitialTargets(target, targetMoves);

            //if start was one move away, get a sequence of the inverse of whatever the last move made was (as comes from the end)
            if (targetCubes.Contains(start)){
                 return new uint[]{GetMoveInverse(targetCubes.GetSequenceOfEqualCube(start)[0])};
            }

            uint[] moves = new uint[]{1,2,3,4,7,8,9,10,13,14,15,16}; // no more F

            //get path from start to any in targetCubes
            return GetPathFromCubeToList(start, targetCubes, GetSubsequentMoves(moves)); 
        }

        public Cube GetThreeGenMaskedCube(Cube cube)
        { //returns a mask of the passed cube containing only values relevant for identifying if a cube can be reached by G_3

            uint[] start = new uint[]{0,0,0,0,0,0};
            uint lastPieceMask = 0xF;

            for (uint face = 0; face<6 ; face++){
                for (int shift = 0; shift <= 24; shift += 8) 
                {
                    //get the next edge of the face
                    uint edge = (cube.GetBitboards()[face] >> shift) & lastPieceMask;

                    //if the current edge is F or B and the related edge is D
                    if ((edge == 6 || edge == 9) && cube.GetRelatedEdge(face, shift) == 10)
                    { 
                        //add the edge to the starting bitboards
                        start[face] = start[face] | (edge << shift);
                    }

                }
            }

            //return the G_3 masked cube equivalent to the one passed
            return new Cube(start);
        }
        
        //TWO GEN

        public uint[] GetTwoGenSolution(Cube cube)
        { //returns a sequence to convert a cube reachable by the generating set G_5 = {U,L,R} to one reachable by G_4 = {U,R}

            //this is a 2x2x3 on the bottom left, it isn't the entire requirement for G-4, but is sufficient when paired with Cube.IsTwoGenIfLeftBlock()
            Cube target = new Cube(new uint[]{0, 0xA, 0, 0, 0x99000, 0x66}); 
            Cube start = GetTwoGenMaskedCube(cube);

            if (start.Equals(target) && cube.IsTwoGenIfLeftBlock()) return new uint[0];

            uint[] targetMoves = new uint[]{3,9,15}; //L Face
            CubeStore targetCubes = GetInitialTargets(target, targetMoves);

            //if start is one move away from a starting point and start rotated by the inverse is in g2
             if (targetCubes.Contains(start) && 
                    start.RotatedByMove(targetCubes.GetSequenceOfEqualCube(start)[0]).IsTwoGenIfLeftBlock()){
                 return new uint[]{GetMoveInverse(targetCubes.GetSequenceOfEqualCube(start)[0])};
            }

             uint[] moves = new uint[]{1,3,4,7,9,10,13,15,16}; // no more D

            //a different function is used here from the one used for other reductions, as there is an additional check that cannot be represented in its entirety by a generic.
            return GetTwoGenPathFromCubeToList(cube, start, targetCubes, GetSubsequentMoves(moves));
        }

        public Cube GetTwoGenMaskedCube(Cube cube)
        { //returns a mask of the passed cube containing only values relevant for identifying if a cube can be reached by G_2

            uint[] start = new uint[]{0,0,0,0,0,0};
            uint lastPieceMask = 0xF;

            for (uint face = 0; face<6 ; face++) {
                for (int shift = 0; shift <= 24; shift += 8) {

                    //get the current edge, and the one related to it
                    uint edge = (cube.GetBitboards()[face] >> shift) & lastPieceMask;
                    uint relatedEdge = cube.GetRelatedEdge(face, shift);

                    //if the current edge is not U and related ede is L, or if the current edge is L and the related isn't U
                    if ((edge != 12 && relatedEdge == 3))
                    {  
                        //add the edge to the starting bitboard
                        start[face] = start[face] | (edge << shift);
                    }

                    //get the current corner. this is the current edge but shifted a further 4 to get to last position on board (faces are stored edge, corner, edge, corner,...)
                    uint corner = (cube.GetBitboards()[face] >> (shift + 4)) & lastPieceMask; 

                    //if corner is DBL or DFL (6 ways for this to happen)
                    if ((corner == 9 && cube.GetClockwiseRelatedCorner(face, shift + 4) == 3) //B clockwise L -> DBL
                            || (corner == 6 && cube.GetClockwiseRelatedCorner(face, shift + 4) == 10)) //F clockwise D -> DFL
                    {  

                        //add the part of the found corner on the current face to the starting bitboard
                        start[face] = start[face] | (corner << (shift + 4));
                    }
                }
            }

            //return a new cube with the starting bitboard;
            return new Cube(start);
        }
        
        public uint[] GetTwoGenPathFromCubeToList(Cube cube, Cube start, CubeStore targetCubes, uint[][] subsequentMoves)
        { //get a path from a starting cube and associated mask to those contained in the starting cube
           
            //this function is identical to GetPathFromCubeToList but with one noted change
              
            CubeStore startCubes = new CubeStore();
            startCubes.Add(start);

            while (true)
            {

                Cube currentCube = startCubes.Dequeue();

                foreach (uint move in subsequentMoves[currentCube.GetLastMove()])
                {
                    Cube newCube = currentCube.RotatedByMove(move);

                    if (startCubes.Add(newCube) && targetCubes.Contains(newCube)) 
                    {
                        //rather than returning the sequence from newCube to an EqualCube immediately, it is also checked to see if it is TwoGen().
                        //this is separate to finding a matched state, as there are MANY to match (8!).

                        uint[] seq = MergeSequences(newCube.GetSequence(), targetCubes.GetSequenceOfEqualCube(newCube));
                        if (cube.CleanRotatedBySequence(seq).IsTwoGenIfLeftBlock()) return seq;
                    } 

                }

                currentCube = targetCubes.Dequeue();

                foreach (uint move in subsequentMoves[currentCube.GetLastMove()]) 
                {
                    Cube newCube = currentCube.RotatedByMove(move);

                    if (targetCubes.Add(newCube) && startCubes.Contains(newCube))
                    {
                        uint[] seq = MergeSequences(startCubes.GetSequenceOfEqualCube(newCube), newCube.GetSequence());
                        if (cube.CleanRotatedBySequence(seq).IsTwoGenIfLeftBlock()) return seq;                        
                    }

                }
                
            }

        }

        //ONE GEN

        public uint[] GetOneGenSolution(Cube cube)
        { //returns a sequence to convert a cube reachable by the generating set G_2 = {U,R} to one reachable by G_1 = {U}

            //the desired cubes will be one turn away from a solved cube
            Cube target = new Cube();

            //the starting cube is taken as a copy() of the passed cube to avoid any previous sequences affecting the process. All pieces are kept, as all are required
            Cube start = cube.CleanCopy();
            
            //if the cube is already solved, return an empty sequence. a state that can be reached by G_0 can naturally also be reached by G_1
            if (start.Equals(target)) return new uint[0];

            //define a new cube store
            CubeStore targetCubes = new CubeStore();

            //define all the moves that could result in a state reachable by G_1 = {U}
            uint[] targetMoves = new uint[]{1,7,13};

            //for each move in moves
            foreach (uint move in targetMoves)
            {
                //add a cube rechable from target by rotating it by this move. this avoids adding the final move to the sequence, but keeps it as the last move. 
                //if it was kept this function would reduce to G_0, not G_1, but without the last move the sequence could go 'through' G_0 and back out again
                targetCubes.Add(target.RotatedWithLastMove(move));
            }

            //adds the original to the cube, i.e. all four possible states reached as transition from G_2 -> G_1. Means that solver will attempt to 
            //readd those added above a second time, but doing it this way keeps all four cubes in G_1 accessible with equal chance. 
            //added below so others are checked first, makes the process feel nicer but has no difference to correctness.
            targetCubes.Add(target);

             //if start is contained in the set of cubes one turn (U face) from solved, it is 1-gen. return an empty sequence.
            if (targetCubes.Contains(start)) return new uint[0];
            
            uint[] moves = new uint[]{1,7,13,4,10,16};
            return GetPathFromCubeToList(start, targetCubes, GetSubsequentMoves(moves));
        }

        //ZERO GEN

        public uint[] GetZeroGenSolution(Cube cube)
        { //returns a sequence to convert a cube reachable by the generating set G_1 = {U} to one reachable by G_0 = {}

            Cube target = new Cube(); 
            Cube start = cube.CleanCopy();

            if (start.Equals(target)) return new uint[0];
            
            uint[] moves = new uint[]{1,7,13}; //all remaining moves

            //for each move in the list of moves
            foreach(uint move in moves)
            {
                //if rotating the passed cube by this moves results in the target cube, return the move
                if (start.RotatedByMove(move).Equals(target))
                {
                    return new uint[]{move};
                }
            }

            //if it wasn't reached, return 0. this won't happen if the cube passed is reachable by G_1
            return new uint[0];
        }

        //SHARED PROCESSES

        public CubeStore GetInitialTargets(Cube target, uint[] moves)
        { //returns a CubeStore populated with the cubes generated by rotating target by each of the moves in moves

            //define a new CubeStore
            CubeStore targetCubes = new CubeStore();

            //for each movein the passed list, get new cube rotated by move (adds that move to the sequence for the cube) and add the cube to list
            foreach (uint move in moves){
                targetCubes.Add(target.RotatedByMove(move));   
            }

            //return the CubeStore
            return targetCubes;
        }
        
        public uint[] GetPathFromCubeToList(Cube start, CubeStore targetCubes, uint[][] subsequentMoves)
        { //finds a path from a starting Cube to a cube stored in targetCubes using the move sequences defined in subsequentMoves

            //define a second CubeStore for the cubes from the start and add the start cube.
            CubeStore startCubes = new CubeStore(); 
            startCubes.Add(start);
            
            //until a path is found
            while (true)
            {

                //remove the next cube in the list of cubes found from the starting point
                Cube currentCube = startCubes.Dequeue();

                    //for each move that can succeed the previous one made on the current cube, determined by subsequentMoves
                foreach (uint move in subsequentMoves[currentCube.GetLastMove()])
                {

                    //get the cube found by rotating the current cube by the new rotation
                    Cube newCube = currentCube.RotatedByMove(move);

                    //if new cube state hasn't been found before, and if the new cube position has been found from the other direction, a path has been found between the two.
                    if (startCubes.Add(newCube) && targetCubes.Contains(newCube))
                    {
                        //merge the sequence that got to the newly added cube, and the reversed sequence taken from the target to the matching position.
                        return MergeSequences(newCube.GetSequence(), targetCubes.GetSequenceOfEqualCube(newCube));
                    }
                }

                currentCube = targetCubes.Dequeue();

                foreach (uint move in subsequentMoves[currentCube.GetLastMove()]) 
                {
                    Cube newCube = currentCube.RotatedByMove(move);

                    if (targetCubes.Add(newCube) && startCubes.Contains(newCube)) 
                    {

                        //note the order of the sequences is different here, as the order of the path is backwards (and so this sequence should be flipped, not the other one)
                        return MergeSequences(startCubes.GetSequenceOfEqualCube(newCube), newCube.GetSequence());
                    }
                }
            
            }

        }

        public uint[] MergeSequences(uint[] forwards, uint[] backwards)
        { //merges two sequences, by concatenating forwards to the inverse of backwards

            //defime a new sequence of the appropriate length, and an index i to iterative through the sequence with
            uint[] sequence = new uint[forwards.Length + backwards.Length];
            int i = 0;

            //get all the moves in forwards, and add them to sequence. increment i accordingly
            foreach (uint move in forwards)
            {
                sequence[i] = move;
                i++;
            }

             //get the move that undoes each move in the backwards sequence, in reverse order (the inverse)
            for (int j = backwards.Length-1;j>=0; j--)
            {
                sequence[i] = GetMoveInverse(backwards[j]);
                i++;
            }

            //return the merged sequence
            return sequence;

            //note process is safe when joining within GetPathFromCubeToList from , for example, R matching R', as before R' was introduced into backwards,
            //  R2 would have found the state before R' in the previous cycle. i.e. no need to check if joins should be combined into one move.
        }

        //MOVE SET AND SEQUENCE GENERATION

        public uint GetMoveInverse(uint move)
        {//returns the inverse of a passed move

            //try get the inverse. return it if found, and return 0 if not found
            try
            {
                return new uint[]{0, 13, 14, 15, 16, 17, 18, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6}[move]; //1-6 mapped to themselves + 12, and the reverse.
            }
            catch
            {
                return 0;
            }
        }

        public uint[] GetScramble(uint size)
        { //returns a scramble that cannot be easily simplified using all 18 moves, of the requested size

            //if the requested size is 0, return an empty array
            if (size == 0) return new uint[0];

            //define a list of all possible moves
            uint[] allMoves = new uint[]{1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18};

            //define a new random generator
            System.Random random = new System.Random();

            //define a new scramble of the required size
            uint[] scramble = new uint[size];

            //add a random starting move to scramble
            scramble[0] = allMoves[random.Next(allMoves.Length)]; 

            //if the requested size was 1, return the curent array (which only contains one index)
            if (size == 1) return scramble;

            //while a new move different to the first has not yet been found
            while (true)
            {
                //get a new move
                uint nextMove = allMoves[random.Next(allMoves.Length)];

                //if the move is not the sane as the first move, add it to the scramble
                if (!((nextMove % 6) == (scramble[0] % 6)))
                { 
                    //break from the loop
                    scramble[1] = nextMove;
                    break;

                }
            }

            //define a new iterator, i, and loop while this is less than the requested size (if it was defined as 2, the loop will skip)
            int i = 2;
            while (i < size)
            {

                //get a new move
                uint nextMove = allMoves[random.Next(allMoves.Length)];

                //if the move is different to the previous
                if (!((nextMove % 6) == (scramble[i-1] % 6))

                        //and not with the previous turn being opposite face and two ago being the same face (L,R,L, etc)
                        && !((nextMove % 2 != 0) && (scramble[i-1] % 6) == (nextMove % 6) + 1 && (scramble[i-2] % 6 == nextMove)) //odd even odd
                        && !((nextMove % 2 == 0) && (scramble[i-1] % 6) == (nextMove % 6) - 1 && (scramble[i-2] % 6 == nextMove))) //even odd even
                {  

                    //add the move to the scramble and increment the index
                    scramble[i] = nextMove;
                    i++;
                }
                
            }

            //return the completed scramble
            return scramble;
        }

        public uint[][] GetSubsequentMoves(uint[] moves)
        { //generates a list of possible subsequent moves from the list of moves passed

            //get the iexes needed for all moves (as a map) (the highest move representated + 1)
            uint numIndexes = NumIndexesNeeded(moves);

            //initialise a 2d array of th eappropriate size to store the moves that will be returned 
            uint[][] subsequentMoves = new uint[numIndexes][];

            //nitialise a list used for the sub-moves of each specific starting move
            List<uint> thisMoveSubMoves = new List<uint>(); 

            //for eah move in the passed array
            foreach (uint move in moves)
            {
                //clear the list of submoves as we have a new move
                thisMoveSubMoves.Clear();

                //for each submove move in moves
                foreach (uint subMove in moves)
                {

                    //if the move isn't the same face as sub-move, and the move isn't even with the previous move being odd (U -> D not allowed, D -> U allowed).
                    if (!(move % 6 == subMove % 6) && !(move % 2 == 0 && move % 6 == subMove % 6 + 1))
                    {
                        //add it to the list of submoves
                        thisMoveSubMoves.Add(subMove);
                    }
                }
                
                //define the array stored at the current subsequentMoves index to be empty and of the appropriate length
                subsequentMoves[move] = new uint[thisMoveSubMoves.Count];

                //add every move in thisMoveSubMoves to the moves stored in the right index of subsequentMoves
                for (int i = 0; i < thisMoveSubMoves.Count;i++)
                {
                    subsequentMoves[move][i] = thisMoveSubMoves[i];
                }
            }

            //define the first index of subsequentMoves to be of the correct length, and copy the most list into it
            subsequentMoves[0] = new uint[moves.Length];
            Array.Copy(moves, subsequentMoves[0], moves.Length);

            //return the 2d array used to map from a move to a list of its subsequent moves
            return subsequentMoves;

        }

        public uint NumIndexesNeeded(uint[] moves)
        { //function determines how many indexes are needed in order to map directly to all elements of a current array (max value + 1). ASSUMES ALL MOVES ARE POSITIVE.

            //one index for 0, the starting move
            if (moves.Length == 0) return 1;

            //get first val
            uint max = moves[0]; 

            //for each move, if the current move is a higher value than any previously found, set this to be the new max
            foreach (uint i in moves)
            {
                if (i > max) max = i;
            }

            //return max +1, as if the max to be represnted is 4, for example, we need 5 indexes (0-4)
            return max + 1; 
        }

    }
}