using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] private float vel = 1;

    // Update is called once per frame
    void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * vel);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * vel);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * vel);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * vel);
        }
    }
}
