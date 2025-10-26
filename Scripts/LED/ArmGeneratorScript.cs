using UnityEngine;

public class ArmGeneratorScript : MonoBehaviour
{
    [SerializeField] int armNumber;
    [SerializeField] GameObject arm;
    [SerializeField] GameObject spike;

    void Start()
    {
        for (int i = 0; i < armNumber; i++)
        {
            Instantiate(arm, this.transform.TransformPoint(new Vector2(i + 1, 0)), this.transform.rotation, this.transform);
        }
        Instantiate(spike, this.transform.TransformPoint(new Vector2(armNumber + 1.5f, 0)), this.transform.rotation, this.transform);
    }
}
