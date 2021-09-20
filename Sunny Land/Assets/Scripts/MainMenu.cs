using UnityEngine;
using UnityEngine.Tilemaps;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private float nbOfUnitsToMoveSky = 0.8f;
    [SerializeField] private float nbOfUnitsToMoveWeeds = 1.3f;

    public Tilemap skyAndSea1;
    public Tilemap skyAndSea2;
    public Tilemap weeds1;
    public Tilemap weeds2;


    private void Update()
    {
        MoveBackground();
    }

    private void MoveBackground()
    {
        skyAndSea1.transform.Translate(Vector3.left * nbOfUnitsToMoveSky * Time.deltaTime);
        skyAndSea2.transform.Translate(Vector3.left * nbOfUnitsToMoveSky * Time.deltaTime);
        weeds1.transform.Translate(Vector3.left * nbOfUnitsToMoveWeeds * Time.deltaTime);
        weeds2.transform.Translate(Vector3.left * nbOfUnitsToMoveWeeds * Time.deltaTime);

        if (skyAndSea1.transform.position.x <= -24)
            skyAndSea1.transform.position = skyAndSea2.transform.position + Vector3.right * 24;
        if (skyAndSea2.transform.position.x <= -24)
            skyAndSea2.transform.position = skyAndSea1.transform.position + Vector3.right * 24;
        if (weeds1.transform.position.x <= -33)
            weeds1.transform.position = weeds2.transform.position + Vector3.right * 33;
        if (weeds2.transform.position.x <= -33)
            weeds2.transform.position = weeds1.transform.position + Vector3.right * 33;
    }

}
