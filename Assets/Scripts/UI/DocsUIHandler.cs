using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DocsUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text nextEditorText;
    [SerializeField] private GameObject loopSticky;

    [SerializeField] private Transform loopVerticalLayout;

    [SerializeField] private Transform contentHolder;
    [SerializeField] private GameObject docButton;

    [SerializeField] private GameObject docPanel;
    [SerializeField] private Image docBackground;

    [SerializeField] private GameObject menuPanel;

    [SerializeField] List<Color> docColors;

    List<GameObject> loopStickies = new List<GameObject>();

    List<Button> docButtons = new List<Button>();

    int currentDocIndex = 0;

    InputManager.ActionMap lastActionMap = InputManager.ActionMap.Player;

    private void OnEnable()
    {
        DocsManager.onDocAdded += OnDocAdded;
    }

    private void OnDisable()
    {
        DocsManager.onDocAdded -= OnDocAdded;
    }

    private void OnDocAdded(Doc doc, int index)
    {
        var button = Instantiate(docButton, contentHolder).GetComponent<Button>();

        button.onClick.AddListener(() => OnDocButtonClicked(index));
        button.GetComponent<Image>().color = docColors[index];

        docButtons.Add(button);
    }

    public void OnDockUIOpened()
    {
        menuPanel.SetActive(true);

        lastActionMap = InputManager.instance.activeActionMap;
        InputManager.instance.SwitchActionMap(InputManager.ActionMap.UI);

        InputManager.onEscape += OnEscape;

        for (int i = 0; i < DocsManager.instance.docs.Count; i++)
        {
            var doc = DocsManager.instance.docs[i];

            docButtons[i].gameObject.SetActive(!doc.taken);

            docButtons[i].transform.GetChild(0).gameObject.SetActive(doc.completed);      //check mark 
        }

        OnDocButtonClicked(currentDocIndex);
    }


    private void OnEscape()
    {
        if (menuPanel.activeInHierarchy)
        {
            InputManager.onEscape -= OnEscape;

            OnDockUIClosed();
        }
    }

    public void OnDockUIClosed()
    {
        menuPanel.SetActive(false);
        InputManager.instance.SwitchActionMap(lastActionMap);
    }

    private void OnDocButtonClicked(int index)
    {
        currentDocIndex = index;

        if (DocsManager.instance.docs.Count > 0 && !DocsManager.instance.docs[currentDocIndex].taken && !DocsManager.instance.docs[currentDocIndex].completed)
        {
            docPanel.SetActive(true);

            docBackground.color = docColors[index];

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
                    sticky.GetComponentInChildren<Image>().gameObject.SetActive(true);      //check mark 
                }

                loopStickies.Add(sticky);
            }

            nextEditorText.text = "Bring to " + doc.nextEditor.name;
        } else
        {
            docPanel.SetActive(false);
        }
    }
}
