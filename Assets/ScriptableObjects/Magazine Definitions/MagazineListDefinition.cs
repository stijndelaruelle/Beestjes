using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Beestjes/Magazine List Definition")]
public class MagazineListDefinition : ScriptableObject
{
    [SerializeField]
    private List<MagazineDefinition> m_MagazineList;

    //Accessors
    public MagazineDefinition GetMagazineDefinition(int magazineID)
    {
        if (m_MagazineList == null)
            return null;

        if (magazineID < 0 || magazineID > m_MagazineList.Count)
            return null;

        return m_MagazineList[magazineID];
    }

    public int GetID(MagazineDefinition magazineDefinition)
    {
        if (m_MagazineList == null)
            return -1;

        if (magazineDefinition == null)
            return -1;

        return m_MagazineList.IndexOf(magazineDefinition);
    }

    public int GetMagazineDefinitionCount()
    {
        if (m_MagazineList == null)
            return -1;

        return m_MagazineList.Count;
    }
}
