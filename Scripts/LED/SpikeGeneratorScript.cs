using UnityEngine;
using UnityEngine.UIElements;

public class SpikeGeneratorScript : MonoBehaviour
{
    [SerializeField] int spikeNumber;
    [SerializeField] GameObject spikeSolo;
    [SerializeField] GameObject spikeLeft;
    [SerializeField] GameObject spikeRight;
    [SerializeField] GameObject spikeMiddle;

    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        if (spikeNumber == 1) Instantiate(spikeSolo, new Vector2(transform.position.x, transform.position.y), this.transform.rotation);
        else if (spikeNumber == 2)
        {
            Instantiate(spikeLeft, new Vector2(transform.position.x, transform.position.y), this.transform.rotation);
            Instantiate(spikeRight, this.transform.TransformPoint(new Vector2(1, 0)), this.transform.rotation);
        }
        else if (spikeNumber > 2)
        {
            Instantiate(spikeLeft, new Vector2(transform.position.x, transform.position.y), this.transform.rotation);
            for (int i = 0; i < spikeNumber - 2; i++)
            {
                Instantiate(spikeMiddle, this.transform.TransformPoint(new Vector2(i + 1, 0)), this.transform.rotation);
            }
            Instantiate(spikeRight, this.transform.TransformPoint(new Vector2(spikeNumber - 1, 0)), this.transform.rotation);
        }
    }
}
