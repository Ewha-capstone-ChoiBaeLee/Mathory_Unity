using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene_Anim : MonoBehaviour
{
    public float animSpeed = 0.1f;
    public Animator animator1;
    public Animator animator2;

    public int animIndex = 0;
    public int gameLevel;

    public string[] imageNames1, imageNames2, imageNames3, imageNames4, imageNames5, imageNames6;

    // Start is called before the first frame update
    void Start()
    {
        gameLevel = PlayerPrefs.GetInt("GameLevel");
        StartCoroutine(DisplayImages(gameLevel));
    }

    // Update is called once per frame
    void Update()
    {
        animator1.speed = animSpeed;
        animator2.speed = animSpeed;
    }

    IEnumerator DisplayImages(int gameLevel)
    {
        // 처음 3초 대기
        yield return new WaitForSeconds(0.5f);

        switch (gameLevel)
        {
            case 1:
                foreach (string img in imageNames1)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(4f); // 3초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (string img in imageNames2)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(4f); // 3초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
            case 3:
                foreach (string img in imageNames3)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f); // 2초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
            case 4:
                foreach (string img in imageNames4)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(4f); // 3초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
            case 5:
                foreach (string img in imageNames5)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f); // 2초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
            case 6:
                foreach (string img in imageNames6)
                {
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f); // 2초 대기
                    GameObject.Find("Canvas/image").transform.Find(img).gameObject.SetActive(false);
                }
                break;
        }
    }
}
