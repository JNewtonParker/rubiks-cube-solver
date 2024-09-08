using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Solver;

public class CubeSolverTests
{

    //MASKED CUBE TESTS

    [Test]
    public void MaskedCubeReturnedFourGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        Cube maskedCube = solver.GetFourGenMaskedCube(cube);
        Cube desiredCube = new Cube(new uint[]{0xF0F0F0F, 0xF0F0F0F, 0, 0, 0xF000F, 0xF000F});

        Assert.AreEqual(maskedCube, desiredCube);
    }

    [Test]
    public void MaskedCubeReturnedThreeGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        Cube maskedCube = solver.GetThreeGenMaskedCube(cube);
        Cube desiredCube = new Cube(new uint[]{0, 0, 0, 0, 0x900, 0x600});

        Assert.AreEqual(maskedCube, desiredCube);
    }

    [Test]
    public void MaskedCubeReturnedTwoGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        Cube maskedCube = solver.GetTwoGenMaskedCube(cube);
        Cube desiredCube = new Cube(new uint[]{0, 0xA, 0, 0, 0x99000, 0x66});

        Assert.AreEqual(maskedCube, desiredCube);
    }

    //BEST CASE TESTING

    [Test]
    public void BestCaseFourGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        CollectionAssert.AreEqual(solver.GetFourGenSolution(cube), new uint[0]);
    }

    [Test]
    public void BestCaseThreeGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        CollectionAssert.AreEqual(solver.GetThreeGenSolution(cube), new uint[0]);
    }

    [Test]
    public void BestCaseTwoGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        CollectionAssert.AreEqual(solver.GetTwoGenSolution(cube), new uint[0]);
    }

    [Test]
    public void BestCaseOneGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        CollectionAssert.AreEqual(solver.GetOneGenSolution(cube), new uint[0]);
    }

    [Test]
    public void BestCaseZeroGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        CollectionAssert.AreEqual(solver.GetZeroGenSolution(cube), new uint[0]);
    }

    //WORST CASE TESTING

    [Test]
    public void WorstCaseFourGen()
    {

        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        cube = cube.RotatedBySequence(new uint[]{2,4,5,1,3,2,6,16,5,13,7}); //worst-case sequence is of length 11

        Assert.NotNull(solver.GetFourGenSolution(cube));
    }

    [Test]
    public void WorstCaseThreeGen()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();
        
        cube = cube.RotatedBySequence(new uint[]{2,4,8,4}); //worst-case sequence is of length 4

        Assert.NotNull(solver.GetThreeGenSolution(cube));
    }

    [Test]
    public void WorstCaseZeroGen()
    {
        //U, U', U2 is every case
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        cube = cube.RotatedByMove(1);

        Assert.NotNull(solver.GetOneGenSolution(cube));
    }

    //PATHING TESTS

    [Test]
    public void PathFromCubeToListFindsPath()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        uint[] moves = new uint[]{1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18};
        CubeStore targetCubes = solver.GetInitialTargets(cube, moves);

        cube = cube.RotatedBySequence(new uint[]{1,2,3,4,5,6});

        Assert.NotNull(solver.GetPathFromCubeToList(cube, targetCubes, solver.GetSubsequentMoves(moves)));
    }

    [Test]
    public void TwoGenPathFromCubeToListFindsPath()
    {
        Cube cube = new Cube();
        CubeSolver solver = new CubeSolver();

        //sequence is arbirtray, made from G_3 = {U,L,R}, i.e. {1,3,4}
        cube = cube.RotatedBySequence(new uint[]{1,3,4,3,1,3,3,4,3,1,4,3,4,4,1,3,4,1,1,1,3,4,3,4,3,3,4,4,4,1}); 
        Cube maskedCube = solver.GetTwoGenMaskedCube(cube);

        uint[] targetMoves = new uint[]{3,9,15};
        CubeStore targetCubes = solver.GetInitialTargets(new Cube(new uint[]{0, 0xA, 0, 0, 0x99000, 0x66}), targetMoves);

        uint[] moves = new uint[]{1,3,4,7,9,10,13,15,16};
        uint[][] subsequentMoves = solver.GetSubsequentMoves(moves);

        Assert.NotNull(solver.GetTwoGenPathFromCubeToList(cube, maskedCube, targetCubes, subsequentMoves));
    }

    //OTHER TESTS

    
    [Test]
    public void MergeSequencesMergesSequences()
    {
        CubeSolver solver = new CubeSolver();

        uint[] sequence1 = new uint[]{1,2,3};
        uint[] sequence2 = new uint[]{18,17,16};

        CollectionAssert.AreEqual(solver.MergeSequences(sequence1, sequence2), new uint[]{1,2,3,4,5,6});
    }

    [Test]
    public void GetScrambleGeneratesScrambleOfCorrectSize()
    {
        CubeSolver solver = new CubeSolver();

        uint[] scramble = solver.GetScramble(18);
        
        Assert.AreEqual(scramble.Length, 18);

    }

    [Test]
    public void GetSubsequentMovesGeneratesCorrectMoves()
    {
        CubeSolver solver = new CubeSolver();

        uint[] moves = new uint[]{1,2,3};
        uint[][] subsequentMoves = solver.GetSubsequentMoves(moves);

        uint[][] desiredMoves = new uint[4][];
        desiredMoves[0] = new uint[]{1,2,3}; //0 -> U, D, L
        desiredMoves[1] = new uint[]{2,3}; //U -> D, L
        desiredMoves[2] = new uint[]{3}; //D -> L (D -!> U as U,D is equivalent to D, U)
        desiredMoves[3] = new uint[]{1,2}; //L -> U, D

        for (int i = 0; i<subsequentMoves.Length; i++)
        {
            for (int j = 0; j < subsequentMoves[i].Length; j++)
            {
                if (subsequentMoves[i][j] != desiredMoves[i][j])
                {
                    Assert.Fail();
                }
            }
        }


    }

    [Test]
    public void NumIndexesNeededReturnsCorrectValueInList()
    {
        CubeSolver solver = new CubeSolver();
        uint[] moves = new uint[]{2,5,5,8,9,0,7};

        Assert.AreEqual(solver.NumIndexesNeeded(moves), 10);
    }

    [Test]
    public void NumIndexesNeededReturnsOneWhenMovesAreEmpty()
    {
        CubeSolver solver = new CubeSolver();
        uint[] moves = new uint[0];

        Assert.AreEqual(solver.NumIndexesNeeded(moves), 1);
    }


}
