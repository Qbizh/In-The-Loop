using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DocsUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text nextEditorText;
    [SerializeField] private GameObject loopSticky;

    [SerializeField] private Transform loopVerticalLayout;

    [SerializeField] private Transform scrollContentHolder;
    [SerializeField] private GameObject docButton;

    [SerializeField] private GameObject docPanel;

    List<GameObject> loopStickies = new List<GameObject>();

    List<Button> docButtons = new List<Button>();

    int currentDocIndex = 0;

    private void OnEnable()
    {
        DocsManager.onDocAdded += OnDocAdded;
    }

    private void OnDisable()
    {
        DocsManager.onDocAdded -= OnDocAdded;
    }

    private void OnDocAdded(Doc doc)
    {
        var button = Instantiate(docButton, scrollContentHolder).GetComponent<Button>();

        button.onClick.AddListener(() => OnDocButtonClicked(docButtons.Count - 1));

        docButtons.Add(button);
    }

    public void OnDockUIOpened()
    {
        for (int i = 0; i < DocsManager.instance.docs.Count; i++)
        {
            var doc = DocsManager.instance.docs[i];

            docButtons[i].gameObject.SetActive(!doc.taken);
        }

        OnDocButtonClicked(currentDocIndex);
    }

    private void OnDocButtonClicked(int index)
    {
        currentDocIndex = index;

        if (!DocsManager.instance.docs[currentDocIndex].taken)
        {
            docPanel.SetActive(true);

            var doc = DocsManager.instance.docs[index];

            foreach (var sticky in loopStickies)
            {
                Destroy(sticky);
            }

            loopStickies.Clear();

            foreach (var npc in doc.loop)
            {
                var sticky = Instantiate(loopSticky, loopVerticalLayout);
                sticky.GetComponentInChildren<TMP_Text>().text = npc.Key.name;

                if (npc.Value)
                {
                    sticky.GetComponent<Image>().color = Color.HSVToRGB(0, 0, 0.5f);
                }

                loopStickies.Add(sticky);
            }

            nextEditorText.text = doc.nextEditor.name;
        } else
        {
            docPanel.SetActive(false);
        }
    }
}
