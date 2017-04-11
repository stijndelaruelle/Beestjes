using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Beestjes/Quest List Definition")]
public class QuestListDefinition : ScriptableObject
{
    [SerializeField]
    private List<QuestDefinition> m_QuestList;

    //Accessors
    public QuestDefinition GetQuestDefinition(int QuestID)
    {
        if (m_QuestList == null)
            return null;

        if (QuestID < 0 || QuestID > m_QuestList.Count)
            return null;

        return m_QuestList[QuestID];
    }

    public int GetID(QuestDefinition QuestDefinition)
    {
        if (m_QuestList == null)
            return -1;

        if (QuestDefinition == null)
            return -1;

        return m_QuestList.IndexOf(QuestDefinition);
    }

    public int GetQuestDefinitionCount()
    {
        if (m_QuestList == null)
            return -1;

        return m_QuestList.Count;
    }
}
