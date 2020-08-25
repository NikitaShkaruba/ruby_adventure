using UnityEngine;

/**
 * Controller for NPC's. Ruby can interact with NPC's, after that they show her their message
 */
public class NonPlayableCharacter : MonoBehaviour
{
    [SerializeField] private float displayTime = 7f;
    [SerializeField] private GameObject dialogBox;
    private float _dialogDisplayTimer;

    private void Start()
    {
        _dialogDisplayTimer = displayTime;
        dialogBox.SetActive(true);
    }

    private void Update()
    {
        if (_dialogDisplayTimer < 0) return;
        
        _dialogDisplayTimer -= Time.deltaTime;
        if (_dialogDisplayTimer < 0)
        {
            dialogBox.SetActive(false);
        }
    }

    public void DisplayDialog()
    {
        _dialogDisplayTimer = displayTime;
        dialogBox.SetActive(true);
    }
}
