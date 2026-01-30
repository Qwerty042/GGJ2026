using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuizMaster : MonoBehaviour
{
    public GameObject questionBoxPrefab;
    public GameObject answerButtonPrefab;

    private GameObject questionBox;
    private Vector3 questionLocation = new Vector3(-4.5f, -2.75f, 0);
    private GameObject[] answerButtons = new GameObject[4];
    private Vector3[] buttonLocations = { new Vector3(2.0f,-2.0f,0),
                                          new Vector3(6.5f,-2.0f,0),
                                          new Vector3(2.0f,-3.5f,0),
                                          new Vector3(6.5f,-3.5f,0)};

    private int correctButtonIndex = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i] = Instantiate(answerButtonPrefab, buttonLocations[i], Quaternion.identity);
            answerButtons[i].GetComponentInChildren<TextMeshPro>().text = "Answer " + (i+1).ToString();
        }
        questionBox = Instantiate(questionBoxPrefab, questionLocation, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if(hit.transform != null && hit.transform.gameObject == answerButtons[i])
            {
                if (Input.GetMouseButton(0))
                {
                    answerButtons[i].GetComponent<SpriteRenderer>().color = (i == correctButtonIndex) ? Color.green : Color.red;
                }
                else
                {
                    answerButtons[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                answerButtons[i].GetComponent<SpriteRenderer>().color = Color.gray7;
            }
        }

    }
}
