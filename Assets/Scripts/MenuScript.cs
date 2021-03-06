﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject titleText, playButton, settingsButton, htpButton, exitButton, btmButton, solveButton,
                       settingsMenu;
    private SolarSystem solarSystem;
    private CameraScript mainCamera;
    private GameObject[] planetInfoPanels;
    ConfirmSettingsScript settingsSwitcher;
    enum UI_Phase
    {
        PlanetInfo,
        SolarSystem,
        MainMenu,
        Settings,
    }
    private UI_Phase currentPhase;
    // Start is called before the first frame update
    void Start()
    {
        titleText = GameObject.Find("3dPuzzleText");
        playButton = GameObject.Find("PlayButton");
        settingsButton = GameObject.Find("SettingsButton");
        settingsMenu = GameObject.Find("SettingsMenu");
        htpButton = GameObject.Find("HowToPlayButton");
        exitButton = GameObject.Find("ExitButton");
        btmButton = GameObject.Find("BackToMenuButton");
        solveButton = GameObject.Find("SolveButton");
        settingsSwitcher = settingsMenu.GetComponent<ConfirmSettingsScript>();
        playButton.GetComponent<Button>().onClick.AddListener(PlayTask);
        settingsButton.GetComponent<Button>().onClick.AddListener(SettingsTask);
        btmButton.GetComponent<Button>().onClick.AddListener(BackTask);
        exitButton.GetComponent<Button>().onClick.AddListener(ExitTask);

        btmButton.SetActive(false);
        solveButton.SetActive(false);
        settingsMenu.SetActive(false);
        solarSystem = GameObject.FindGameObjectWithTag("Sun").GetComponent<SolarSystem>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

        planetInfoPanels = GameObject.FindGameObjectsWithTag("InfoPanel");
        foreach (GameObject infoPanel in planetInfoPanels)
        {
            infoPanel.SetActive(false);
        }

        currentPhase = UI_Phase.MainMenu;
    }
    void PlayTask()
    {
        titleText.SetActive(false);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);

        solarSystem.EnableSolarSystemPhase(true);
        mainCamera.GoFree();
        currentPhase = UI_Phase.SolarSystem;
    }
    void BackTask()
    {
        Debug.Log("Back clicked");
        switch (currentPhase)
        {
            case UI_Phase.SolarSystem:
                StartCoroutine(_WaitForCameraMenuPhase());
                btmButton.SetActive(false);
                solarSystem.EnableSolarSystemPhase(false);
                mainCamera.GoMenu();
                break;
            case UI_Phase.PlanetInfo:
                mainCamera.GoFree();
                currentPhase = UI_Phase.SolarSystem;
                foreach (GameObject infoPanel in planetInfoPanels)
                {
                    infoPanel.SetActive(false);
                }
                solveButton.SetActive(false);
                break;
            case UI_Phase.Settings:
                StartCoroutine(_WaitForCameraMenuPhase());
                settingsSwitcher.ResetSwitchers();
                btmButton.SetActive(false);
                settingsMenu.SetActive(false);
                mainCamera.GoMenu();
                break;
        }
    }
    void SettingsTask()
    {
        mainCamera.GoSettings();
        titleText.SetActive(false);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        htpButton.SetActive(false);
        exitButton.SetActive(false);
        btmButton.SetActive(true);
        StartCoroutine(_WaitForCameraSettingsPhase());
    }
    void ExitTask()
    {
        Application.Quit(0);
    }
    IEnumerator _WaitForCameraMenuPhase()
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.Menu))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        titleText.SetActive(true);
        playButton.SetActive(true);
        settingsButton.SetActive(true);
        htpButton.SetActive(true);
        exitButton.SetActive(true);
        currentPhase = UI_Phase.MainMenu;
        yield break;
    }
    IEnumerator _WaitForCameraSettingsPhase()
    {
        while (!mainCamera.isCurrentPhase(CameraScript.Phase.Settings))// IsCurrentPhaseMenu())
        {
            yield return new WaitForSeconds(0.01f);
        }
        currentPhase = UI_Phase.Settings;
        settingsMenu.SetActive(true);
        yield break;
    }
    IEnumerator _WaitForCameraLock(uint focusedPlanetInex)
    {
        while(!mainCamera.isCurrentPhase(CameraScript.Phase.PlanetLock))// IsCurrentPhasePlanetLock())
        {
            yield return new WaitForSeconds(0.01f);
        }
        foreach (GameObject infoPanel in planetInfoPanels)
        {
            if (infoPanel.transform.GetComponent<PlanetInfoScript>().GetIndex() == focusedPlanetInex)
            {
                infoPanel.SetActive(true);
            }
        }
        currentPhase = UI_Phase.PlanetInfo;
        btmButton.SetActive(true);
        solveButton.SetActive(true);
        yield break;
    }
   
    void _HandlePlanetClick()
    {
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began) && mainCamera.isCurrentPhase(CameraScript.Phase.Free)) //ReadyForLock())
        {
            Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                if (raycastHit.transform.gameObject.tag == "Planet")
                {
                    mainCamera.FocusOn(raycastHit.transform.gameObject);
                    StartCoroutine(_WaitForCameraLock(raycastHit.transform.gameObject.GetComponent<PlanetScript>().GetIndex()));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case UI_Phase.SolarSystem:
                _HandlePlanetClick();
                break;
        }
    }
}
