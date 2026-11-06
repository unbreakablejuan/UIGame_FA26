using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Keeps the Enter button disabled until the player has typed a name.
public class NameGate : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button submitButton;

    void Start()
    {
        // start disabled, since the field begins empty
        submitButton.interactable = false;

        // runs every time the text in the field changes
        nameInput.onValueChanged.AddListener(OnNameChanged);
    }

    void OnNameChanged(string text)
    {
        // Trim() removes spaces, so "   " does not count as a name
        submitButton.interactable = text.Trim().Length > 0;
    }
}
