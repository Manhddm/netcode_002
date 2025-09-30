using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 3;
    [SerializeField] private int maxNameLength = 12;
    public const string PLayerNameKey = "PlayerName"; 
    private void Start()
    {

        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        
        
        nameField.text = PlayerPrefs.GetString(PLayerNameKey, string.Empty);
        HandleChangeName();
    }
    public void HandleChangeName()
    {
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
        
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PLayerNameKey, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
