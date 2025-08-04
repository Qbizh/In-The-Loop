using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeBoard : MonoBehaviour
{
    [SerializeField] Camera headshotCam;
    [SerializeField] Transform hairHolder;

    [SerializeField] Transform grid;

    public void SetUp(List<NPC> npcs)
    {
        headshotCam.gameObject.SetActive(true);

        for (int i = 0; i < grid.childCount; i++) 
        {
            var card = grid.GetChild(i);

            if (!card.CompareTag("Boss"))
            {
                var npc = npcs[i - 1];

                card.GetComponentInChildren<TMP_Text>().text = npc.name;

                var hair = Instantiate(npc.hair, hairHolder);
                hair.GetComponent<Renderer>().material = npc.hairColor;

                var rt = new RenderTexture(200, 200, 16);
                headshotCam.targetTexture = rt;
                headshotCam.Render();

                hair.SetActive(false);
                Destroy(hair);

                card.GetComponentInChildren<RawImage>().texture = rt;
            }
        }

        headshotCam.gameObject.SetActive(false);
    }
}
