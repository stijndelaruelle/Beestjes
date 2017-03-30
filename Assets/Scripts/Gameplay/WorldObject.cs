using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

//Trees, bushes, benches, etc... anything that a user can place
public class WorldObject : MonoBehaviour
{
    [SerializeField]
    private List<AnimalSlot> m_AnimalSlots;

    [Tooltip("Lifetime in minutes")]
    [SerializeField]
    private int m_LifeTime;
    private DateTime m_EndTime; //Time when this object grows or decays

    public void Serialize()
    {
        foreach (AnimalSlot animalSlot in m_AnimalSlots)
            animalSlot.Serialize();
    }

    public void Deserialize(JSONClass worldObjectNode)
    {
        //Time when
        JSONNode placeTimeNode = worldObjectNode["end_time"];
        if (placeTimeNode != null)
        {
            try
            {
                m_EndTime = DateTime.ParseExact(placeTimeNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("A save game \"world_object\" has an invalid \"end_time\" node! Expected DateTime. Source: " + placeTimeNode.ToString() + " Exception: " + e.Message);
            }
        }

        //Position
        float posX = worldObjectNode["position_x"].AsFloat;
        float posY = worldObjectNode["position_y"].AsFloat;
        float posZ = worldObjectNode["position_z"].AsFloat;

        transform.position = new Vector3(posX, posY, posZ);

        //Scale
        bool isFlipped = worldObjectNode["is_flipped"].AsBool;
        float xScaleModifier = 1.0f;
        if (isFlipped) xScaleModifier *= -1.0f;

        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * xScaleModifier, scale.y, scale.z);


        //Animal slots
        JSONArray animalSlotsNode = worldObjectNode["animal_slots"].AsArray;
        if (animalSlotsNode != null)
        {
            for (int i = 0; i < animalSlotsNode.Count; ++i)
            {
                JSONClass animalSlotNode = animalSlotsNode[i].AsObject;

                if (i < m_AnimalSlots.Count)
                {
                    m_AnimalSlots[i].Deserialize(animalSlotNode);
                }
                else
                {
                    throw new System.Exception("A save game \"world_object\" has more animal slots than the prefab holds!");
                }
            }
        }
    }
}
