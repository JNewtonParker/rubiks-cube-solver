using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Solver;

public class CubeTests
{

    //CONSTRUCTOR TESTS

    [Test]
    public void NewCubeIsSolved()
    {
        Cube cube = new Cube();
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x33333333, 0x55555555, 0x99999999, 0x66666666});
    }

    [Test]
    public void NewCubeMatchesBitboards()
    {
        //rotated before test so it doesnt just check if it matches solved.
        Cube cube = new Cube();
        cube = cube.RotatedByMove(1);

        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
    }

    //COPYING TESTS

    [Test]
    public void CleanCopyReturnsCleanCopy()
    {
        Cube cube = new Cube();
        cube = cube.RotatedByMove(1);
    
        Cube copy = cube.CleanCopy();

        CollectionAssert.AreEqual(copy.GetBitboards(), cube.GetBitboards());
        CollectionAssert.AreEqual(copy.GetSequence(), new uint[0]);
        Assert.AreEqual(copy.GetLastMove(), 0);
    }

    // //ROTATION RESULT TESTS

    [Test]  
    public void CleanRotatedBySequenceCleanRotatesBySequence()
    {
        uint[] sequence = new uint[]{13,13,13};

        Cube cube = new Cube();
        cube = cube.CleanRotatedBySequence(sequence);

        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
        CollectionAssert.AreEqual(cube.GetSequence(), new uint[0]);
        Assert.AreEqual(cube.GetLastMove(), 0);
    }
    
    [Test]  
    public void RotatedBySequenceRotatesBySequence()
    {
        uint[] sequence = new uint[]{13,13,13};

        Cube cube = new Cube();
        cube = cube.RotatedBySequence(sequence);

        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
        CollectionAssert.AreEqual(cube.GetSequence(), sequence);
        Assert.AreEqual(cube.GetLastMove(), 13);
    }

    [Test]  
    public void RotatedWithLastMoveRotatesByLastMove()
    {
        Cube cube = new Cube();
        cube = cube.RotatedWithLastMove(1);

        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
        CollectionAssert.AreEqual(cube.GetSequence(), new uint[0]);
        Assert.AreEqual(cube.GetLastMove(), 1);
    }

    [Test]  
    public void RotatedByMoveRotatesByMove()
    {
        Cube cube = new Cube();
        cube = cube.RotatedByMove(1);

        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
        CollectionAssert.AreEqual(cube.GetSequence(), new uint[]{1});
        Assert.AreEqual(cube.GetLastMove(), 1);
    }
       
    // //SINGLE ROTATION TESTS

    [Test]
    public void RotatedByU1()
    {
        Cube cube = new Cube().RotatedByMove(1);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x66633333, 0x99955555, 0x33399999, 0x55566666});
    }

    [Test]
    public void RotatedByU2()
    {
        Cube cube = new Cube().RotatedByMove(7);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x55533333, 0x33355555, 0x66699999, 0x99966666});
    }

    [Test]
    public void RotatedByU3()
    {
        Cube cube = new Cube().RotatedByMove(13);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x99933333, 0x66655555, 0x55599999, 0x33366666});
    }

    [Test]
    public void RotatedByD1()
    {
        Cube cube = new Cube().RotatedByMove(2);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x33339993, 0x55556665, 0x99995559, 0x66663336});
    }

    [Test]
    public void RotatedByD2()
    {
        Cube cube = new Cube().RotatedByMove(8);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x33335553, 0x55553335, 0x99996669, 0x66669996});
    }

    [Test]
    public void RotatedByD3()
    {
        Cube cube = new Cube().RotatedByMove(14);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCCCCC, 0xAAAAAAAA, 0x33336663, 0x55559995, 0x99993339, 0x66665556});
    }

    [Test]
    public void RotatedByL1()
    {
        Cube cube = new Cube().RotatedByMove(3);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0x9CCCCC99, 0x6AAAAA66, 0x33333333, 0x55555555, 0x99AAA999, 0xC66666CC});
    }

    [Test]
    public void RotatedByL2()
    {
        Cube cube = new Cube().RotatedByMove(9);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xACCCCCAA, 0xCAAAAACC, 0x33333333, 0x55555555, 0x99666999, 0x96666699});
    }

    [Test]
    public void RotatedByL3()
    {
        Cube cube = new Cube().RotatedByMove(15);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0x6CCCCC66, 0x9AAAAA99, 0x33333333, 0x55555555, 0x99CCC999, 0xA66666AA});
    }
    
    [Test]
    public void RotatedByR1()
    {
        Cube cube = new Cube().RotatedByMove(4);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCC666CCC, 0xAA999AAA, 0x33333333, 0x55555555, 0xC99999CC, 0x66AAA666});
    }

    [Test]
    public void RotatedByR2()
    {
        Cube cube = new Cube().RotatedByMove(10);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCAAACCC, 0xAACCCAAA, 0x33333333, 0x55555555, 0x69999966, 0x66999666});
    }

    [Test]
    public void RotatedByR3()
    {
        Cube cube = new Cube().RotatedByMove(16);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCC999CCC, 0xAA666AAA, 0x33333333, 0x55555555, 0xA99999AA, 0x66CCC666});
    }
    
    [Test]
    public void RotatedByB1()
    {
        Cube cube = new Cube().RotatedByMove(5);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0x555CCCCC, 0xAAAA333A, 0xC33333CC, 0x55AAA555, 0x99999999, 0x66666666});
    }

    [Test]
    public void RotatedByB2()
    {
        Cube cube = new Cube().RotatedByMove(11);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xAAACCCCC, 0xAAAACCCA, 0x53333355, 0x55333555, 0x99999999, 0x66666666});
    }

    [Test]
    public void RotatedByB3()
    {
        Cube cube = new Cube().RotatedByMove(17);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0x333CCCCC, 0xAAAA555A, 0xA33333AA, 0x55CCC555, 0x99999999, 0x66666666});
    }
    
    [Test]
    public void RotatedByF1()
    {
        Cube cube = new Cube().RotatedByMove(6);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCC333C, 0x555AAAAA, 0x33AAA333, 0xC55555CC, 0x99999999, 0x66666666});
    }

    [Test]
    public void RotatedByF2()
    {
        Cube cube = new Cube().RotatedByMove(12);
        
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCCAAAC, 0xCCCAAAAA, 0x33555333, 0x35555533, 0x99999999, 0x66666666});
    }

    [Test]
    public void RotatedByF3()
    {
        Cube cube = new Cube().RotatedByMove(18);
     
        CollectionAssert.AreEqual(cube.GetBitboards(), new uint[]{0xCCCC555C, 0x333AAAAA, 0x33CCC333, 0xA55555AA, 0x99999999, 0x66666666});
    }

    // //TWO GEN TESTS
    
    [Test]
    public void IsTwoGenIfLeftBlockIfTwoGen()
    {
        Cube cube = new Cube();
        Assert.True(cube.IsTwoGenIfLeftBlock());
    }

    [Test]
    public void IsNotTwoGenIfLeftBlockIfNotTwoGen()
    {
        //sequence maintains left block and makes two gen not two gen (not invertible)
        uint[] sequence = new uint[]{4,13,16,1,16,13,3};
        Cube cube = new Cube();
        cube = cube.RotatedBySequence(sequence);

        Assert.False(cube.IsTwoGenIfLeftBlock());
    }

    // //EQUALITY TESTS

    [Test]
    public void equalCubesWithSameSequenceAreEqual()
    {
        Cube cube = new Cube();
        cube = cube.RotatedByMove(1);

        Cube otherCube = new Cube();
        otherCube = otherCube.RotatedByMove(1);

        Assert.AreEqual(cube, otherCube);
    }

    [Test]
    public void equalCubesWithDifferentSequencesAreEqual()
    {
        Cube cube = new Cube();
        cube = cube.RotatedByMove(1); //equivalent
        
        Cube otherCube = new Cube();
        otherCube = otherCube.RotatedBySequence(new uint[]{13,13,13}); //equivalent

        Assert.AreEqual(cube, otherCube);
    }

}
