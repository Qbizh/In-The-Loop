using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<NPC> coworkers;
    public NPC boss;

    [Header("Settings")]
    public float talkSpeed = 10f;
    public float editingTime = 3f;
    public float readTime = 1f;

    [Header("Refrences")]

    [SerializeField] List<string> names;
    [SerializeField] List<GameObject> hairs;
    [SerializeField] List<Material> hairColors;

    [SerializeField] EmployeeBoard employeeBoard;

    List<(GameObject, Material)> hairCombos = new List<(GameObject, Material)>();

    public static NPCManager instance;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        } else
        {
            Destroy(this);
        }

        foreach (var hair in hairs)
        {        
            foreach (var color in hairColors)
            {
                hairCombos.Add((hair, color));
            }
        }

        Shuffle(hairCombos);
    }

    private void Start()
    {
        foreach (NPC npc in coworkers) 
        {
            var newName = names[Random.Range(0, names.Count)];
            var hair = hairCombos[0];

            if (newName != null)
            {
                names.Remove(newName);
                hairCombos.Remove(hair);

                npc.SetUp(newName, hair.Item1, hair.Item2);
            }
        }

        employeeBoard.SetUp(coworkers);
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
