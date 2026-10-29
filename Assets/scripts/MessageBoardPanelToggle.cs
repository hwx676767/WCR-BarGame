using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MessageBoardPanelToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject messageBoard;

    [SerializeField]
    private string messageBoardRootName = "MessageBoard";

    private Button toggleButton;

    private void Awake()
    {
        ResolveMessageBoardRoot();
        toggleButton = GetComponent<Button>();
        toggleButton.onClick.RemoveListener(ToggleMessageBoard);
        toggleButton.onClick.AddListener(ToggleMessageBoard);
    }

    private void Start()
    {
        SetMessageBoardVisible(false);
    }

    public void Configure(GameObject board)
    {
        messageBoard = board;
    }

    public void ToggleMessageBoard()
    {
        ResolveMessageBoardRoot();

        if (messageBoard != null)
            SetMessageBoardVisible(!messageBoard.activeSelf);
    }

    private void SetMessageBoardVisible(bool visible)
    {
        ResolveMessageBoardRoot();

        if (messageBoard != null)
            messageBoard.SetActive(visible);
    }

    private void ResolveMessageBoardRoot()
    {
        GameObject namedRoot = FindGameObjectByName(messageBoardRootName);
        if (namedRoot != null && namedRoot != gameObject)
            messageBoard = namedRoot;
    }

    private static GameObject FindGameObjectByName(string objectName)
    {
        foreach (Transform transform in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (transform.name == objectName)
                return transform.gameObject;
        }

        return null;
    }
}
