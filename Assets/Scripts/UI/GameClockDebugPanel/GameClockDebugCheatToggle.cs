using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class GameClockDebugCheatToggle : MonoBehaviour
{
    private Toggle m_Toggle;

    private void Awake()
    {
        m_Toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
        {
            gameClock.IsCheatingChangedEvent += OnIsCheatingChanged;
            Refresh(gameClock.IsCheating);
        }
    }

    private void OnDestroy()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
            gameClock.IsCheatingChangedEvent -= OnIsCheatingChanged;
    }

    private void Refresh(bool isCheating)
    {
        if (m_Toggle.isOn != isCheating)
            m_Toggle.isOn = isCheating;
    }

    public void Toggle()
    {
        GameClock.Instance.SetCheating(m_Toggle.isOn);
    }

    //Events
    private void OnIsCheatingChanged(bool isCheating)
    {
        Refresh(isCheating);
    }
}
