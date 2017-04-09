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

    private DateTime m_EndTime; //Time until no new animal will spawn

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Reset();
    }

    private void Reset()
    {
        SetCurrentAnimal(null);
        m_EndTime = DateTime.MinValue;
    }

    public void Tick(DateTime dateTime, PartOfDay partOfDay, float percentageOfPODPassed)
    {
        //If we are still occupied, nothing happens
        UpdateAnimalPrecense();
        if (HasAnimal())
            return;

        //2 lists that run in sync
        List<AnimalDefinition> availableAnimals = new List<AnimalDefinition>();
        List<float> stayTimes = new List<float>();

        //Collect all the animals that are able to spawn
        foreach (AnimalDefinition animal in m_AnimalDefinitions)
        {
            float stayTime = 0.0f;
            animal.CanSpawn(partOfDay, percentageOfPODPassed, true, out stayTime);
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

        m_EndTime = dateTime.AddMinutes(stayTimes[randID]);
    }

    private void UpdateAnimalPrecense()
    {
        if (m_CurrentAnimal == null)
            return;

        DateTime dateTime = GameClock.Instance.GetDateTime();

        //If the animal ran away
        if (dateTime >= m_EndTime)
        {
            Reset();
            return;
        }

        //If the animal doesn't reveal itself at this time
        float percentageOfPODPassed = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, dateTime, ref percentageOfPODPassed);

        bool canSpawn = m_CurrentAnimal.CanSpawn(partOfDay, percentageOfPODPassed, false);

        if (canSpawn == false)
        {
            Reset();
            return;
        }
    }

    private void SetCurrentAnimal(AnimalDefinition animal)
    {
        m_CurrentAnimal = animal;

        if (m_CurrentAnimal == null)
            m_SpriteRenderer.sprite = null;
        else
            m_SpriteRenderer.sprite = m_CurrentAnimal.Sprite;
    }

    public int CalculatePictureScore(List<string> tagList, Rect cameraRect)
    {
        int score = 0;

        if (HasAnimal() == false)
            return score;

        //Overlap ratio of our own sprite renderer
        Rect viewPortRect = m_SpriteRenderer.GetViewportRect();
        float ratio = viewPortRect.GetOverlapPercentage(cameraRect);

        if (ratio > 0.0f)
        {
            score += Mathf.FloorToInt(m_CurrentAnimal.Score * ratio);

            if (tagList.Contains(m_CurrentAnimal.name) == false)
            {
                score += m_CurrentAnimal.FirstScoreBonus;
                tagList.Add(m_CurrentAnimal.name);
            }
        }

        return score;
    }

    public bool HasAnimal()
    {
        return (m_CurrentAnimal != null);
    }

    public void Serialize(JSONClass animalSlotNode)
    {
        int currentAnimalID = m_AnimalDefinitions.IndexOf(m_CurrentAnimal);
        animalSlotNode.Add("animal_id", new JSONData(currentAnimalID));
        animalSlotNode.Add("end_time", new JSONData(m_EndTime.ToString("dd/MM/yyyy HH:mm:ss")));
    }

    public void Deserialize(JSONClass animalSlotNode)
    {
        //Animal ID
        int currentAnimalID = animalSlotNode["animal_id"].AsInt;

        //No animal is present
        if (currentAnimalID == -1)
        {
            Reset();
            return;
        }

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
                m_EndTime = DateTime.ParseExact(placeTimeNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("A save game \"animal slot\" has an invalid \"end_time\" node! Expected DateTime. Source: " + placeTimeNode.ToString() + " Exception: " + e.Message);
            }
        }

        UpdateAnimalPrecense();
    }
}
