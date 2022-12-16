using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public GameObject MapMenuItem;
    public GameObject MapMenu;
    public TMP_InputField MapNameInputField;

    public static UiManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void FillMapMenu()
    {
        foreach (Transform item in MapMenu.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (var map in CubeGenerator.Instance.Maps.Maps)
        {
            var obj = Instantiate(MapMenuItem, MapMenu.transform);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = map.name;
        }
    }

    public void ToggleMapMenu()
    {
        MapMenu.gameObject.SetActive(!MapMenu.activeSelf);
    }
}