using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    private UIManager ui;
    private Transform cursor;

    private Text aboveText;
    public string[] remarks;

    public string[] dialog;
    private bool finishedDialog;
    private int dialogProgress;

    private bool hover;
    private float a;

    private float killText;

    void Awake()
    {
        a = 0;

        dialogProgress = 0;
        finishedDialog = false;

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();

        aboveText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();

        transform.GetChild(0).gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        cursor = GameObject.Find("Cursor").transform;

        aboveText.text = "Press RMB to interact";
    }

    void Update()
    {
        aboveText.gameObject.transform.position = new Vector3(aboveText.gameObject.transform.position.x, aboveText.gameObject.transform.position.y + Mathf.Sin(a) / 600, aboveText.gameObject.transform.position.z);
        a += 0.03f;

        if (Vector3.Distance(transform.position, cursor.position) < 1.25f && ui.inGame)
        {
            hover = true;
        }
        else
        {
            hover = false;
        }

        if (hover && Input.GetMouseButtonDown(1))
        {
            if (ui.session.achievements[8] == false)
            {
                StartCoroutine(ui.Achievement(8));
                Debug.Log("Achievement Get!");
            }

            killText = Random.Range(1, 10);

            if (!finishedDialog)
            {
                aboveText.text = dialog[dialogProgress];
                dialogProgress++;
                StartCoroutine(TextFade());

                if (dialogProgress >= dialog.Length)
                {
                    finishedDialog = true;

                    GameObject dropItem = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[7], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    dropItem.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Data").GetComponent<GameData>().items[48].sprite;

                    dropItem.GetComponent<DroppedItem>().inventory = GameObject.Find("Player").GetComponent<OpenMenu>();
                    dropItem.GetComponent<DroppedItem>().hotbar = GameObject.Find("Player").GetComponent<Hotbar>();
                    dropItem.GetComponent<DroppedItem>().itemId = 48;
                    dropItem.GetComponent<DroppedItem>().count = 1;

                    GameObject.Find("WorldGenerator").GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                    GameObject.Find("WorldGenerator").GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(dropItem);

                    dropItem.GetComponent<DroppedItem>().Initialize();
                }
            }
            else
            {
                aboveText.text = remarks[Random.Range(1, remarks.Length) - 1];
                StartCoroutine(TextFade());
            }
        }
    }

    IEnumerator TextFade()
    {
        float killCheck = killText;

        yield return new WaitForSeconds(1.5f);

        if (killText == killCheck)
        {
            aboveText.text = null;
        }
    }
}
