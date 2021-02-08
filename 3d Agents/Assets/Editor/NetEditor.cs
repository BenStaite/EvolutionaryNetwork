using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 [@CustomEditor(typeof(NetHandler))]
public class NetEditor : Editor
{

    public NeuralNetwork[] nets;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        //Called whenever the inspector is drawn for this object.
        DrawDefaultInspector();
        //This draws the default screen.  You don't need this if you want
        //to start from scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.
        if (GUILayout.Button("Save Best"))
        {
            
            //add everthing the button would do.
        }
    }
}