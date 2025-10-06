using UnityEngine;
using UnityEngine.UI;

public class SceneManagerVN : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    public void ChangeBackground(Sprite newBg)
    {
        backgroundImage.sprite = newBg;
    }
}