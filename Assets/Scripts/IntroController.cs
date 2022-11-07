using UnityEngine;

public class IntroController : MonoBehaviour
{
    public GameObject playbutton;

    // Start is called before the first frame update
    void Start()
    {
        iTween.MoveBy(playbutton, Vector3.one * .1f, Time.deltaTime);
    }
}
