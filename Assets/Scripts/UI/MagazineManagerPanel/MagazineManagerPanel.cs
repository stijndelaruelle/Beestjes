using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineManagerPanel : MonoBehaviour
{
    [SerializeField]
    private MagazineManager m_MagazineManager;

    [SerializeField]
    private MagazineDisplayPanel m_MagazinePanelPrefab;
    private List<MagazineDisplayPanel> m_MagazinePanels;

    [SerializeField]
    private RectTransform m_Content;

    private void Awake()
    {
        m_MagazinePanels = new List<MagazineDisplayPanel>();
    }

    private void OnEnable()
    {
        List<Magazine> magazines = m_MagazineManager.Magazines;

        if (magazines == null)
            return;

        for (int i = 0; i < magazines.Count; ++i)
        {
            if (i >= m_MagazinePanels.Count)
            {
                AddMagazinePanel(magazines[i]);
            }
            else
            {
                m_MagazinePanels[i].Initialize(magazines[i]);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (MagazineDisplayPanel magazinePanel in m_MagazinePanels)
        {
            GameObject.Destroy(magazinePanel.gameObject);
        }

        m_MagazinePanels.Clear();
    }

    private void AddMagazinePanel(Magazine magazine)
    {
        MagazineDisplayPanel newPanel = GameObject.Instantiate<MagazineDisplayPanel>(m_MagazinePanelPrefab, m_Content);
        newPanel.Initialize(magazine);

        m_MagazinePanels.Add(newPanel);
    }
}
