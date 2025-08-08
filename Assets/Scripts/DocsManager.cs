using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static NPC;

public class DocsManager : MonoBehaviour
{
    public static DocsManager instance;
    public static event Action<Doc, int> onDocAdded;

    public List<Doc> docs = new List<Doc>();

    [SerializeField] int maxLoopLength = 6;

    public int editorsNecessary = 5;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public void GenerateDoc()
    {
        Doc doc = new Doc();
        doc.loop.Add(NPCManager.instance.boss, true);
        GetNextEditor(doc);

        docs.Add(doc);

        onDocAdded?.Invoke(doc, docs.Count - 1);
    }

    public NPC GetNextEditor(Doc doc)
    {
        var npcs = new List<NPC>(NPCManager.instance.coworkers);
        NPCManager.Shuffle(npcs);

        foreach (var npc in npcs)
        {
            if (!doc.editHistory.Contains(npc) && GetRelevantDocs(npc, true).Count == 0 && GetRelevantDocs(npc, false).Count == 0)
            {
                doc.nextEditor = npc;
                doc.editHistory.Add(npc);

                npc.status = NPC.NPCStatus.Waiting;

                return npc;
            }
        }

        doc.nextEditor = null;
        return null;
    }

    public bool TryEditDoc(NPC npc, Doc doc)
    {
        if (!doc.taken && doc.nextEditor == npc && !doc.NeedsReview())
        {
            doc.taken = true;
            return true;
        }

        return false;
    }

    public List<Doc> GetRelevantDocs(NPC npc, bool forReview)
    {
        var list = new List<Doc>();

        foreach (var doc in docs) 
        {
            if (forReview)
            {
                if (doc.loop.ContainsKey(npc) && !doc.loop[npc])
                {
                    list.Add(doc);
                }
            } else
            {
                if (doc.nextEditor == npc)
                {
                    list.Add(doc);
                }
            }
        }

        return list;
    }

    public void FinishEdit(Doc doc)
    {
        doc.taken = false;

        if (doc.editHistory.Count <= editorsNecessary && !doc.completed)
        {
            foreach (var npc in doc.loop.Keys.ToList())
            {
                if (npc != doc.editHistory[doc.editHistory.Count - 2])      // get the editor that just made an edit - if in loop don't get them to review it
                {
                    doc.loop[npc] = false;
                    npc.SetStatus(NPCStatus.Reviewing);
                }
            }
        } else
        {
            CompleteDoc(doc);
        }
    }

    private void CompleteDoc(Doc doc)
    {
        doc.completed = true;

        bool allCompleted = true;

        foreach (var d in docs)
        {
            if (!d.completed)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            GameManager.instance.EndGame(true);
        }
    }

    public bool AddToLoop(NPC npc, Doc doc)
    {
        if (doc.loop.Count < maxLoopLength)
        {
            int random = UnityEngine.Random.Range(0, 3);
            if (random == 0)
            {
                doc.loop.Add(npc, true);
                return true;
            }
        }

        return false;
    }

    public void ReviewDoc(NPC npc, Doc doc)
    {
        if (doc.loop.ContainsKey(npc))
        {
            doc.loop[npc] = true;
        }
    }
}
