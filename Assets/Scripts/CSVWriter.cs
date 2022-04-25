using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVWriter
{
    public string FileName { get; private set; }
    public TextWriter writer { get; private set; }

    /**
     * Prepares a csv file with a given filename
     */
    public CSVWriter(string filename)
    {
        FileName = Application.persistentDataPath + "/" + filename + "test.csv";
        writer = new StreamWriter(FileName, true);
        writer.WriteLine("Time; X; Y; Z");
    }

    /**
     * Serilizes the data to the CSV file 
     */
    public void serilize(SensorData data)
    {
        writer.WriteLine($"{data.Miliseconds}; {data.X}; {data.Y}; {data.Z}");
    }
}
