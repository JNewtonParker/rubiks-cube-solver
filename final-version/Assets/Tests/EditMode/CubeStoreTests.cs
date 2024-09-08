using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Solver;

public class CubeStoreTests
{

    //ADDING TESTS

    [Test]
    public void NewCubeIsAdded()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();

        Assert.True(cubeStore.Add(cube));
    }

    [Test]
    public void RepeatedCubeIsNotAdded()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();
    
        cubeStore.Add(cube);

        Assert.False(cubeStore.Add(cube));
    }

    //REMOVAL TESTS

    [Test]
    public void DequeueReturnsCubeWhenNotEmpty()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();

        cubeStore.Add(cube);

        Assert.NotNull(cubeStore.Dequeue());
    }

    //CONTAINS TESTS

    [Test]
    public void ContainedCubeReturnsTrue()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();

        cubeStore.Add(cube);

        Assert.True(cubeStore.Contains(cube));
    }


    [Test]
    public void NotContainedCubeReturnsFalse()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();

        Assert.False(cubeStore.Contains(cube));
    }

    //RETRIVAL TESTS

    [Test]
    public void SequenceForValidCubeReturned()
    {
        CubeStore cubeStore = new CubeStore();
        Cube cube = new Cube();

        cubeStore.Add(cube);

        Assert.NotNull(cubeStore.GetSequenceOfEqualCube(cube));
    }
}