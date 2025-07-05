using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    public Text text;
    [SerializeField] private Text[] charas;
    [SerializeField] private RectTransform[] Start_Pos;
    [SerializeField] private RectTransform[] End_Pos;

    void Start()
    {
        for (int i = 0; i < charas.Length; i++)
        {

            charas[i].transform.DOPath(new Vector3[] {
                        Start_Pos[i].position,
                        MidPoint(Start_Pos[i].position, End_Pos[i].position, 30f),
                        End_Pos[i].position },
             1f,
             PathType.CatmullRom).SetDelay(i * 0.1f).SetEase(Ease.InOutSine);
        }
    }

    Vector3 MidPoint(Vector3 start, Vector3 end, float height)
    {
        Vector3 mid = (start + end) / 2f;
        mid.y += height; // Y•ûŒü‚É”g‚ðì‚é
        return mid;
    }
}
