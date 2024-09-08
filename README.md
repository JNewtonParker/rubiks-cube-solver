## Dissertation Abstract

With over 450 million sold worldwide, Rubik’s Cubes are the most sold puzzle to date. Consisting of 26 small cubes that move freely through rotation of each face, the aim is to align the pieces such that each face of the cube contains only a single colour. This project introduces a method for solving such a puzzle, sequentially restricting which faces a cuber – someone that solves Rubik’s Cubes – is allowed to use for the remainder of the solving process. For example, a cuber may be allowed to rotate five of six sides initially, but in stage two of solving, they may only rotate four. Methods of solving exist that utilise subsets of rotations at different stages of the solving process, but these convert between different subgroups, not necessarily decrementing the total number of rotations available.

An implementation of this method is presented for a predefined order of restriction, before a revised and optimised version is introduced that greatly enhances the performance. In addition, a means of visualising the method is implemented, providing explanations of the method, the underlying concept of restrictions, and a 3D-Model of a cube on which the method can be performed.

The visualisation is evaluated through two waves of user-testing, and is found to be fit for purpose. An investigation is made to find an acceptable worst-case number of cubes stored both concurrently and overall, for all possible orders in which rotation can be restricted. Finally, implementations for each possible order of restriction are ranked based on the two worst-case metrics, first by concurrently stored permutations, then the total stored overall.

## Running the Project

Navigate to the visualisation folder, and run the file CubeTutorial.exe. This runs the project.

Controls, and a light explanation, can be found after pressing the help button.

## Viewing the Source Code

To view the code for the solver, instead navigate through the directories working-versions\finalversion\ Assets\Solver, where the C# files are contained. To run the project in a development
environment, install Unity, and open the final-version folder. The tests for the implementation
can also be run from here.
