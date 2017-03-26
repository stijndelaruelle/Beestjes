using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class AnimalSlot : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private List<AnimalDefinition> m_AnimalDefinitions;
    private AnimalDefinition m_CurrentAnimal;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = null;

        m_CurrentAnimal = null;
    }

    private void Start()
    {
        PickAnimal();
    }

    private void PickAnimal()
    {
        //Cache this somewhere, this really only needs to be calculated once per gametick (15 min)
        float percentage = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, DateTime.Now, ref percentage);

        List<AnimalDefinition> availableAnimals = new List<AnimalDefinition>();

        //Collect all the animals that are able to spawn
        foreach(AnimalDefinition animal in m_AnimalDefinitions)
        {
            if (animal.CanSpawn(partOfDay, percentage))
                availableAnimals.Add(animal);
        }

        //If none are found this slot remains unoccupied
        if (availableAnimals.Count == 0)
        {
            m_SpriteRenderer.sprite = null;
            m_CurrentAnimal = null;
            return;
        }

        //Else we pick from the available list
        int randID = UnityEngine.Random.Range(0, availableAnimals.Count);
        m_CurrentAnimal = availableAnimals[randID];
        m_SpriteRenderer.sprite = m_CurrentAnimal.Sprite;
    }

}
