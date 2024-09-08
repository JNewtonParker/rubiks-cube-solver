using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Solver; //for translator
using TMPro; //for labels

public class TextHandler : MonoBehaviour
{

    public List<GameObject> labels; //labels to write to

    Dictionary<string, string> display; //content for labels

    Translator translator;

    void Start()
    {
        display = new Dictionary<string, string>();
        translator = new Translator();
    }

    public void Clear()
    {
        foreach (GameObject label in labels)
        {
           label.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void Write(string label, uint[] sequence)
    {
        switch (label)
        {
            case "NEW":
                if (labels[0].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[0].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
            case "G4":
                
                if (labels[1].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[1].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
            case "G3":
                
                if (labels[2].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[2].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
            case "G2":
                
                if (labels[3].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[3].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
            case "G1":
                
                if (labels[4].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[4].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
            case "G0":
                
                if (labels[5].GetComponent<TextMeshProUGUI>().text.Equals(""))
                {
                    labels[5].GetComponent<TextMeshProUGUI>().text = translator.GetStringOfSequence(sequence);
                }
                break;
        }
            UpdateSetLabel(label);
    }

    public void UpdateSetLabel(string label)
    {
        switch (label)
        {
            case "NEW":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved from here with only the U, R, L, D, and F faces.";
                break;
            case "G4":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved from here with only the U, R, L, and D faces.";
                break;
            case "G3":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved from here with only the U, R, and L faces.";
                break;
            case "G2":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved from here with only the U and R faces.";
                break;
            case "G1":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved from here with only the U face.";
                break;
            case "G0":
                labels[6].GetComponent<TextMeshProUGUI>().text = "The Cube can be solved with no turns (because it is solved!)";
                break;

        }
    }
}
