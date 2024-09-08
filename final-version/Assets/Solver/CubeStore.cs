using System;
using System.Collections;
using System.Collections.Generic;

namespace Solver
{
    public class CubeStore
    {

        Queue<Cube> queue; //defines a queue for cubes to be added to/removed from
        CubeTree cubeTree; //defines a true to store the sequences

        public CubeStore()
        {//constructor method

            queue = new Queue<Cube>();
            cubeTree = new CubeTree();
        }

        public bool Add(Cube cube)
        { //adds a new cube to the store, returns true if successful

            //add it to the tree. if this was successful,
            if (cubeTree.Add(cube))
            {
                //also add it to the queue. return true.
                queue.Enqueue(cube);
                return true;
            }

            //if it wasn't added to the store, return false
            return false;
        }

        public Cube Dequeue() 
        {//removes a cube from the queue.

            return queue.Dequeue();

            // note that it won't be removed from the tree. this is because the code may attempt to add the cube back into the store again, but this is unnecessary 
            //as the path that does this will never result in finding a solution before another, as all cubes reachable from here will have already been checked elsewhere
        }

        public bool Contains(Cube cube)
        { //returns true if the cube being checked is contained in the tree (i.e. has been added to the queue at some point)

            return (cubeTree.Contains(cube));
        }
        
        public uint[] GetSequenceOfEqualCube(Cube cube)
        { //returns the sequence stored to reach the state of the passed cube

            return cubeTree.GetSequenceFromBitboards(cube.GetBitboards());

            //note that this assumes the tree contains the cube. an execution error will occur otherwise.
        }

        public class CubeTree
        {

            Dictionary<uint, CubeNode> tree; //defines a dictionary mapping from the U face bitboard of a cube, to a node containing the rest of the information for cubes with that U face

            public CubeTree()
            { //initialise  tree dictionary
                tree = new Dictionary<uint, CubeNode>();
            }

            public bool Add(Cube cube) 
            { //add a cube the tree

                //if the tree already contains a cube with a U face that matches the cube being added
                if (tree.ContainsKey(cube.GetBitboards()[0]))
                {

                    //return the result of adding the new cube to the cube node stored at that index
                    return tree[cube.GetBitboards()[0]].Add(cube.GetBitboards(), cube.GetSequence(), 1);
                }


                //if the U face bitboard hasn't yet been used, add a new CubeNode() representing this. return true, as the cube cannot have already been in the tree
                tree[cube.GetBitboards()[0]] = new CubeNode(cube.GetBitboards(), cube.GetSequence(), 1);
                return true;   
            }

            public bool Contains(Cube cube)
            { //return true if the node at the index matching the U face also contains the passed cube

                //returns true if the tree contains an index for the U face of the matching cube, and if the node at the U face index also contains the rest of the cube
                return tree.ContainsKey(cube.GetBitboards()[0]) && tree[cube.GetBitboards()[0]].Contains(cube.GetBitboards(), 1);

                //note && is used over & as it will short circuit the expression if the first condition is false
                //this is needed as otherwise the function would call a method on a null pointer/index 
            }

            public uint[] GetSequenceFromBitboards(uint[] bitboards)
            { //returns the sequence stored at the end of the path resulting from the given bitboard array

                return tree[bitboards[0]].GetSequence(bitboards, 1);

                //note that there is a prerequisite here that this.Contains() has already been called!
            }

            public class CubeNode
            {

                Dictionary<uint, uint[]> sequences; //stores the faces that are used to reach the sequence (mapping is from final face bitboard to sequence)
                Dictionary<uint, CubeNode> children; // branch nodes (next tier bitboard -> related CubeNode)

                public CubeNode(uint[] bitboards, uint[] sequence, uint tier)
                { //recursive definition of CubeNodes to represent the rest of the cube

                    //if this node represents a fifth face (all faces now represented in tree)
                    if (tier == 5)
                    {
                        //initialise sequences, and add it's sequence to the list of sequences reached from the face
                        sequences = new Dictionary<uint, uint[]>();
                        sequences[bitboards[RelevantIndex(tier)]] = sequence;

                    } 
                    //otherwise this is a branch node
                    else 
                    {
                        
                        //add a new dictionary to store the children of this node, and add a new node representing the children found from the passed bitbords
                        children = new Dictionary<uint, CubeNode>();
                        children[bitboards[RelevantIndex(tier)]] = new CubeNode(bitboards, sequence, tier+1);

                    }

                }

                public bool Add(uint[] bitboards, uint[] sequence, uint tier)
                {//recursive finding of last side of the cube that has been found previously

                    //if this node represents a fifth face, final face will be intiialised with the info for the last side
                    if (tier == 5) 
                    {
                        
                        //if sequences contains a key matching the new bitboards
                        if (sequences.ContainsKey(bitboards[RelevantIndex(tier)]))
                        {
                            //return false, as the cube has not been added (as it has been found before)
                            return false;
                        }

                        //otherwise this is a new cube. add it to the list of final faces and return true
                        sequences[bitboards[RelevantIndex(tier)]] = sequence;
                        return true;      
                    }

                    if (children.ContainsKey(bitboards[RelevantIndex(tier)])) //if a cube with sides matching from 0 to current tier already inserted
                    { 
                        return children[bitboards[RelevantIndex(tier)]].Add(bitboards, sequence, tier+1); //add the rest of the cube to that child
                    
                    }     

                    children[bitboards[RelevantIndex(tier)]] = new CubeNode(bitboards, sequence, tier+1); //else no cube matches current side, so add from here
                    return true;
                    
                }

                public bool Contains(uint[] bitboards, uint tier)
                {//returns true if the node contains the bitboards passed

                    //if finalface has been initialised
                    if (tier == 5)
                    {

                        //return true if it contains the Relevant face
                        return sequences.ContainsKey(bitboards[RelevantIndex(tier)]);
                    }


                    //otherwise return true if the current tier bitboard has been found before, and if the node contained at that index also contains the Relevant bitboards
                    return 
                        children.ContainsKey(bitboards[RelevantIndex(tier)]) && 
                        children[bitboards[RelevantIndex(tier)]].Contains(bitboards, tier + 1);  
                }

                public uint[] GetSequence(uint[] bitboards, uint tier)
                {//returns the sequence found at the location of the passed bitboard. assumes index exists.

                    //if final tier
                    if (tier == 5)
                    {
                        //return the sequence found at the index of the final bitboard passed
                        return sequences[bitboards[RelevantIndex(tier)]];
                    }

                    //otherwise return the result of calling this function on the CubeNode at the Relevant index of children
                    return children[bitboards[RelevantIndex(tier)]].GetSequence(bitboards, tier + 1);     
                }

                public uint RelevantIndex(uint tier)
                { //returns the index that should be used at the current tier. 

                    switch (tier) 
                    {   
                        case 0:
                            return 0; //U - changing this one will affect tree variable in CubeTree!
                        case 1:
                            return 3; //R
                        case 2:
                            return 5; //F
                        case 3:
                            return 4; //B
                        case 4:
                            return 2; //L
                        case 5:
                            return 1; //D
                    }
                    throw new Exception("Impossible Tier accessed");

                    //note that this is used to avoid as many unnecessary checks/traversal as possible (later stages, D, L more likely to match and so should be checked later)
                }
               
            }        
        }
    }
}