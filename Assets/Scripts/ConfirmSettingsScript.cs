﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ConfirmSettingsScript : MonoBehaviour
{
    [SerializeField]
    private DificultySwitcherScript.Difficulty curDif = DificultySwitcherScript.Difficulty.Normal;
    [SerializeField]
    private SoundSwitcherScript.Sound curSoundStatus = SoundSwitcherScript.Sound.On;

    private SoundSwitcherScript soundSwitcher;
    private DificultySwitcherScript dificultySwitcher;
    private GameObject confirmButton;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        soundSwitcher = GameObject.Find("SoundSwitch").GetComponent<SoundSwitcherScript>();
        dificultySwitcher = GameObject.Find("DificultySwitch").GetComponent<DificultySwitcherScript>();
        confirmButton = GameObject.Find("ConfirmSettingsButton");
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();

        confirmButton.GetComponent<Button>().onClick.AddListener(ConfirmSettings);

        _UpdateGlobal();
    }
    
    private string _SoundToString(SoundSwitcherScript.Sound sound)
    {
        string translated = "";
        switch (sound)
        {
            case SoundSwitcherScript.Sound.On:
                translated = "On";
                break;
            case SoundSwitcherScript.Sound.Off:
                translated = "Off";
                break;
        }
        return translated;
    }

    private string _DificultyToString(DificultySwitcherScript.Difficulty difficulty)
    {
        string translated = "";
        switch (difficulty)
        {
            case DificultySwitcherScript.Difficulty.Easy:
                translated = "Easy";
                break;
            case DificultySwitcherScript.Difficulty.Normal:
                translated = "Normal";
                break;
            case DificultySwitcherScript.Difficulty.Hard:
                translated = "Hard";
                break;
        }
        return translated;
    }

    public void ResetSwitchers()
    {
        soundSwitcher.ResetTitle(curSoundStatus);
        dificultySwitcher.ResetTitle(curDif);
    }

    private void _UpdateGlobal()
    {
        switch (curSoundStatus)
        {
            case SoundSwitcherScript.Sound.On:
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                break;
            case SoundSwitcherScript.Sound.Off:
                audioSource.Stop();
                break;
        }
    }

    private void ConfirmSettings()
    {
        curDif = dificultySwitcher.GetChosenDifficulty();
        curSoundStatus = soundSwitcher.GetSoundStatus();
        Debug.Log("Sound to confirm:" + _SoundToString(curSoundStatus));
        Debug.Log("Dificulty to confirm:" + _DificultyToString(curDif));
        _UpdateGlobal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
