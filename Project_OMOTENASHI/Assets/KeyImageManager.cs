using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyImageManager : MonoBehaviour
{
    [System.Serializable]
    public class KeyImageEntry
    {
        public KeyCode key;                 // ‘ÎÛƒL[
        public Image targetImage;          // UI—pImage
        public Sprite normalSprite;        // ‰Ÿ‚µ‚Ä‚¢‚È‚¢‚Æ‚«
        public Sprite pressedSprite;       // ‰Ÿ‚µ‚Ä‚¢‚é‚Æ‚«
    }

    [SerializeField]
    private List<KeyImageEntry> keyImageEntries = new List<KeyImageEntry>();

    void Update()
    {
        foreach (var entry in keyImageEntries)
        {
            if (entry.targetImage == null) continue;

            bool isPressed = Input.GetKey(entry.key);
            entry.targetImage.sprite = isPressed ? entry.pressedSprite : entry.normalSprite;
        }
    }
}
