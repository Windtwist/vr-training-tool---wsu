using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResetSaveProgress : MonoBehaviour
{

    public void ResetProgress()
    {

        string path = Application.persistentDataPath + "/json saves/";
        // Small Cuts
        using (var sw = new StreamWriter(path + "SmallCuts.json", false))
        {
            sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
        }

        // Large Wounds
        using (var sw = new StreamWriter(path + "LargeWounds.json", false))
        {
            sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
        }

        // CPR
        using (var sw = new StreamWriter(path + "CPR.json", false))
        {
            sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
        }

        // Head Trauma
        using (var sw = new StreamWriter(path + "HeadTrauma.json", false))
        {
            sw.Write("{\"testingUnlocked\":false,\"trainingSave\":false,}");
        }
        //Heimlich
        using (var sw = new StreamWriter(path + "Heimlich.json", false))
        {
            sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
        }
    }

}
