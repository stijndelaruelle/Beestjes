using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class AnimalSlot : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private List<AnimalDefinition> m_AnimalDefinitions;
    private AnimalDefinition m_CurrentAnimal;

    private DateTime m_TimeOccupied; //Time until no new animal will spawn

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = null;

        m_CurrentAnimal = null;
    }

    public void PickAnimal(DateTime dateTime, PartOfDay partOfDay, float percentageOfPODPassed)
    {
        //If we are still occupied, nothing happens
        if (dateTime < m_TimeOccupied)
            return;

        //2 lists that run in sync
        List<AnimalDefinition> availableAnimals = new List<AnimalDefinition>();
        List<float> stayTimes = new List<float>();

        //Collect all the animals that are able to spawn
        foreach (AnimalDefinition animal in m_AnimalDefinitions)
        {
            float stayTime = animal.CanSpawn(partOfDay, percentageOfPODPassed);
            if (stayTime > 0.0f)
            {
                availableAnimals.Add(animal);
                stayTimes.Add(stayTime);
            }
        }

        //If none are found this slot remains unoccupied
        if (availableAnimals.Count == 0)
        {
            SetCurrentAnimal(null);
            return;
        }

        //Else we pick from the available list
        int randID = UnityEngine.Random.Range(0, availableAnimals.Count);
        SetCurrentAnimal(availableAnimals[randID]);

        m_TimeOccupied = dateTime.AddMinutes(stayTimes[randID]);
    }

    private void SetCurrentAnimal(AnimalDefinition animal)
    {
        m_CurrentAnimal = animal;

        if (m_CurrentAnimal == null)
            m_SpriteRenderer.sprite = null;
        else
            m_SpriteRenderer.sprite = m_CurrentAnimal.Sprite;
    }


    public void Serialize()
    {

    }

    public void Deserialize(JSONClass animalSlotNode)
    {
        //Animal ID
        int currentAnimalID = animalSlotNode["animal_id"].AsInt;
        if (currentAnimalID < 0 || currentAnimalID >= m_AnimalDefinitions.Count)
        {
            throw new System.Exception("A save game \"animal slot\" contains an invalid id!");
        }

        SetCurrentAnimal(m_AnimalDefinitions[currentAnimalID]);

        //End timestamp
        JSONNode placeTimeNode = animalSlotNode["end_time"];
        if (placeTimeNode != null)
        {
            try
            {
                m_TimeOccupied = DateTime.ParseExact(placeTimeNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("A save game \"animal slot\" has an invalid \"end_time\" node! Expected DateTime. Source: " + placeTimeNode.ToString() + " Exception: " + e.Message);
            }
        }
    }
}
