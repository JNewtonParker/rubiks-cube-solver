using System;
using System.Collections;
using System.Collections.Generic;

namespace Solver
{
    public class Translator
    {
        //defines a map of moves from index (move) to the value matching that move in standard cube notation
        string[] moves = new string[]
            {
                "0","U", "D", "L", "R", "B", "F",
                "U2", "D2", "L2", "R2", "B2", "F2",
                "U'", "D'", "L'", "R'", "B'", "F'"
            };


        public string MoveToString(uint move)
        {//returns the standard move notation relating to the uint move representation used internally

            //return the map at a given index if it exists, otherwise return the empty string
            return (move > 0 && move < moves.Length) ? moves[move] : "";
        }

        public string[] SequenceToString(uint[] uintSequence)
        { //returns a sequence of moves in standard move notation relating to the passed uint array

            //define a string array to be returned of the appropriate length
            string[] sequence = new string[uintSequence.Length];

            //for each move, add the string equivalent to the list in the correct place
            for (int i = 0; i < uintSequence.Length; i++)
            {
                sequence[i] = MoveToString(uintSequence[i]);
            }

            //return the resulting sequence
            return sequence;
        }

        public Dictionary<string, string[]> SolutionToString(Dictionary<string, uint[]> uintSolution)
        {//returns the standard move notation relating to the passed string->uint dictionary of moves

            //define a dictioanry to be returned
            Dictionary<string, string[]> solution = new Dictionary<string, string[]>();

            //for each key value pair from the passed dictionary
            foreach (KeyValuePair<string, uint[]> kvp in uintSolution)
            {

                //add a new record into the dictionary mapping from the original key to the correcty notated string equivalent to the original uint[] array
                solution[kvp.Key] = SequenceToString(kvp.Value);
            }

            //return the resulting dictionary
            return solution;
        }

        public string GetStringOfSequence(uint[] sequence)
        {
            string[] stringSequence = SequenceToString(sequence);

            if (stringSequence.Length == 0) return "No turns needed!";
        
            string s = stringSequence[0];

            for (int i = 1; i <stringSequence.Length; i++)
            {
                s += ", ";
                s += stringSequence[i];
            }

            return s;
        }

    }
}