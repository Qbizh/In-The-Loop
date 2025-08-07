using System.Collections.Generic;
using UnityEngine;

public class Doc
{
    public List<NPC> editHistory = new List<NPC>();
    public Dictionary<NPC, bool> loop = new Dictionary<NPC, bool>();

    public NPC nextEditor;

    public bool taken;

    public bool completed = false;

    public bool NeedsReview()
    {
        int count = 0;

        foreach (bool reviewed in loop.Values)
        {
            if (!reviewed)
            {
                count++;
            }
        }

        return count > 0;
    }
}
