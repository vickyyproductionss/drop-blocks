using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
//using Firebase.Database;
//using Firebase.Extensions;

public class GameManager2 : MonoBehaviour
{
    [SerializeField]
    int intensity;
    [SerializeField]
    int intensity2;
    public Material CstmPrefab;
    string Rewarded_Ad_Id = "Rewarded_Android";
    public Sprite goldenBlock1;
    public Sprite goldenBlock2;
    public Sprite goldenBlock3;
    public Sprite goldenBlock4;
    public static GameManager2 instance;
    public GameObject particleSysParent;
    public GameObject particleSysPrefab;
    public GameObject mergeBlockPrefab;
    public GameObject BlockParent;
    public GameObject gameOverPanel;
    public GameObject MainGameObject;
    public GameObject deadline_;
    public GameObject spawnPointsParent;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject pauseMenu;
    public TMP_Text scoreText;
    public TMP_Text HighscoreText;
    public GameObject fallingBlock;
    public GameObject highlightingStripsParent;
    public AudioClip fx1;
    public AudioClip fx2;
    public AudioClip fx3;
    public GameObject audioParent;
    public GameObject audioSourcePrefab;
    public Color[] colors;
    int count = 1;
    float stoppedSeconds = 0;
    public bool spawnNow = false;
    public int score;
    public bool CheckCollisionNow = false;
    public bool isGameOver = false;
    public bool isMerging = false;
    bool checkSpwncndn = false;
    bool isSlided = false;
    //DatabaseReference reference;
    public string[] tournamentName;
    public int tournamentTypes;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        score = 0;
        Time.timeScale = 1;
        isGameOver = false;
        arrangeSize();
        NextBlockValue = Random.Range(1, 6);
        SpawnBlock();
        UpdateScore();
        
    }
    public void gameOver()
    {
        if(score > PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", score);
            PlayfabManager.Instance.SubmitScore(score);
        }
        GoogleAdMobController.instance.ShowInterstitialAd();
        gameOverPanel.SetActive(true);
    }
    void arrangeSize()
    {
        float height = Screen.height;
        float width = Screen.width;
        float d_Height = 1920;
        float d_Width = 1080;
        float d_ratio = d_Height / d_Width;
        float ratio = height / width;
        float newSize = d_ratio/ratio;
        MainGameObject.transform.localScale = new Vector3(newSize, newSize, newSize);
        mergeBlockPrefab.transform.localScale = new Vector3(newSize * 0.98f, newSize * 1, newSize * 0.98f);
    }
    public void playThisSound(AudioClip clip)
    {
        GameObject audioSource = Instantiate(audioSourcePrefab, audioParent.transform);
        AudioSource source = audioSource.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(audioSource, 2);
    }
    private void Update()
    {
        if (fallingBlock)
        {
            if (fallingBlock.tag == "fallingBlock")
            {
                CheckTouch();
            }
        }
    }
    public void pauseGame()
    {
        isSlided = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void ResumeGame()
    {
        isSlided = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void mergeNow(GameObject fallingBlock)
    {
        int mergedBlocks = 0;
        int spawnOrNot = 1;
        int myVal = int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
        for(int i = 0; i < BlockParent.transform.childCount; i++)
        {
            GameObject targetBlock = BlockParent.transform.GetChild(i).gameObject;
            if (targetBlock != fallingBlock)
            {
                float pos_xT = targetBlock.transform.position.x;
                float pos_yT = targetBlock.transform.position.y;
                float pos_xF = fallingBlock.transform.position.x;
                float pos_yF = fallingBlock.transform.position.y;
                float difference_x = pos_xF - pos_xT;
                float difference_y = pos_yF - pos_yT;
                if (Mathf.Abs(difference_x) < 0.5f)
                {
                    if(targetBlock.transform.position.y < fallingBlock.transform.position.y)
                    {
                        float t_y = targetBlock.transform.position.y;
                        float f_y = fallingBlock.transform.position.y;
                        float difference = f_y - t_y;
                        if(Mathf.Abs(difference) < 1.5)
                        {
                            int targetVal = int.Parse(targetBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
                            if(targetVal == myVal)
                            {
                                fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = (int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text) + targetVal).ToString();
                                Vector3 Pos = new Vector3(fallingBlock.transform.position.x, fallingBlock.transform.position.y, -2);
                                
                                changeColorAccordingly(2 * targetVal, fallingBlock);
                                GameObject FX = Instantiate(particleSysPrefab, Pos, Quaternion.identity);
                                FX.GetComponent<ParticleSystem>().startColor = fallingBlock.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
                                FX.transform.parent = particleSysParent.transform;
                                Destroy(FX, 3);
                                changeTextSize(2 * targetVal, fallingBlock);
                                vibrateNow(50);
                                Destroy(targetBlock);
                                score += myVal;
                                UpdateScore();
                                spawnOrNot = 0;
                                mergedBlocks++;
                                playThisSound(fx2);
                            }
                            else
                            {
                                spawnOrNot *= spawnOrNot;
                            }
                        }
                    }
                }
                else if (Mathf.Abs(difference_y) < 0.5f)
                {
                    float t_x = targetBlock.transform.position.x;
                    float f_x = fallingBlock.transform.position.x;
                    float difference = f_x - t_x;
                    if (Mathf.Abs(difference) < 1.5)
                    {
                        int targetVal = int.Parse(targetBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
                        if (targetVal == myVal)
                        {
                            fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = (int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text) + targetVal).ToString();
                            Vector3 Pos = new Vector3(fallingBlock.transform.position.x, fallingBlock.transform.position.y, -2);
                            
                            changeColorAccordingly(2 * targetVal, fallingBlock);
                            GameObject FX = Instantiate(particleSysPrefab, Pos, Quaternion.identity);
                            FX.GetComponent<ParticleSystem>().startColor = fallingBlock.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
                            FX.transform.parent = particleSysParent.transform;
                            Destroy(FX, 3);
                            vibrateNow(50);
                            changeTextSize(2 * targetVal, fallingBlock);
                            Destroy(targetBlock);
                            spawnOrNot = 0;
                            score += myVal;
                            UpdateScore();
                            mergedBlocks++;
                            playThisSound(fx2);
                        }
                        else
                        {
                            spawnOrNot *= spawnOrNot;
                        }
                    }
                }
            }
        }
        if(mergedBlocks == 2)
        {
            fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = (int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text) + myVal).ToString();
            score += myVal;
            changeColorAccordingly((int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text)), fallingBlock);
            if (fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<Text>())
            {
                changeTextSize((int.Parse(fallingBlock.transform.GetChild(0).GetChild(0).GetComponent<Text>().text)), fallingBlock);
            }
            UpdateScore();
        }
        if(spawnOrNot == 1 && fallingBlock.tag != "stableBlock")
        {
            isMerging = false;
            fallingBlock.tag = "stableBlock";
            Invoke("SpawnBlock", 0.5f);
        }
        else if(fallingBlock.tag == "stableBlock")
        {
            isMerging = false;
        }
        else if (spawnOrNot == 0)
        {
            isMerging = false;
        }
        
    }
    void vibrateNow(int timeinMillSec)
    {
        Vibration.Init();
        Vibration.Vibrate(timeinMillSec);
    }
    public string fromIntToString(int value1)
    {
        float value = value1;
        string returnValue = "0";
        if (value < 10000)
        {
            returnValue = value.ToString();
        }
        else if (value >= 10000 && value < 1000000)
        {
            returnValue = (value / 1000).ToString("F2") + "K";
        }
        else if (value >= 1000000 && value < 1000000000)
        {
            returnValue = (value / 1000000).ToString("F2") + "M";
        }
        else if (value > 1000000000)
        {
            returnValue = (value / 1000000000).ToString("F2") + "M";
        }
        return returnValue;
    }
    public void changeColorAccordingly(float value, GameObject objectWithSpriterRenderer)
    {
        if(value == 2)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[0]*intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[0]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if(value == 4)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[1] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[1]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 8)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[2] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[2]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 16)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[3] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[3]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 32)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[4] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[4]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 64)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[5] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[5]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 128)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[6] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[6]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 256)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[7] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[7]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 512)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[8] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[8]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else if (value == 1024)
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[9] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[9]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
        else
        {
            Material NM = CstmPrefab;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material = NM;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.color = colors[9] * intensity2;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", colors[9]);
            Color clr = objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
            clr *= intensity;
            objectWithSpriterRenderer.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", clr);
        }
    }
    public void UpdateScore()
    {
        scoreText.text = "Score: " + fromIntToString(score);
        if(score<PlayerPrefs.GetInt("Highscore"))
        {
            HighscoreText.text = "Highscore: " + fromIntToString(PlayerPrefs.GetInt("Highscore"));
        }
        else
        {
            HighscoreText.text = "Highscore: " + fromIntToString(score);
        }
    }
    void CheckTouch()
    {
        if(Input.touchCount > 0 && !isGameOver && !isMerging && !isSlided)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                var pos = touch.position;
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
                if(hitInfo && SlideOrNot(fallingBlock, Camera.main.ScreenToWorldPoint(pos), hitInfo))
                {
                    if (hitInfo.collider.tag == "1" || hitInfo.collider.tag == "stableBlock")
                    {
                        Vector2 newPos = new Vector2(hitInfo.collider.gameObject.transform.position.x, fallingBlock.transform.position.y);
                        fallingBlock.transform.position = newPos;
                        isSlided = true;
                        string name = hitInfo.collider.gameObject.name;
                        int index = 0;
                        if (hitInfo.collider.tag == "stableBlock")
                        {
                            string count = hitInfo.collider.gameObject.transform.position.x.ToString("#");
                            if (count != "")
                            {
                                index = int.Parse(count);
                            }
                            else
                            {
                                index = 0;
                            }
                            index += 2;
                        }
                        else
                        {
                            index = int.Parse(name);
                        }
                        highlightSlices(index);
                        fallingBlock.GetComponent<Rigidbody2D>().gravityScale = 2;
                    }
                }
                else
                {
                    StartCoroutine(blinkWalls());
                }
            }
        }
    }
    bool SlideOrNot(GameObject fallingBlock, Vector2 TouchPos, RaycastHit2D hitInfo)
    {
        for(int i = 0; i < BlockParent.transform.childCount - 1; i++)
        {
            if(BlockParent.transform.GetChild(i).transform.position.x >= fallingBlock.transform.position.x && BlockParent.transform.GetChild(i).transform.position.x <= TouchPos.x)
            {
                if(BlockParent.transform.GetChild(i).transform.position.y + 0.7f > fallingBlock.transform.position.y)
                {
                    return false;
                }
            }
            else if(BlockParent.transform.GetChild(i).transform.position.x <= fallingBlock.transform.position.x && BlockParent.transform.GetChild(i).transform.position.x >= TouchPos.x)
            {
                if (BlockParent.transform.GetChild(i).transform.position.y + 0.7f > fallingBlock.transform.position.y)
                {
                    return false;
                }
            }
            else if(BlockParent.transform.GetChild(i).transform.position.x == hitInfo.collider.gameObject.transform.position.x && BlockParent.transform.GetChild(i).transform.position.y + 0.7f > fallingBlock.transform.position.y)
            {
                return false;
            }
        }
        return true;
    }
    IEnumerator blinkWalls()
    {
        leftWall.GetComponent<Animator>().enabled = true;
        rightWall.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(2);
        leftWall.GetComponent<Animator>().enabled = false;
        rightWall.GetComponent<Animator>().enabled = false;
        leftWall.GetComponent<SpriteRenderer>().color = Color.gray;
        rightWall.GetComponent<SpriteRenderer>().color = Color.gray;
    }
    void highlightSlices(int index)
    {
        highlightingStripsParent.transform.GetChild(index).GetComponent<SpriteRenderer>().color = fallingBlock.GetComponent<SpriteRenderer>().material.GetColor("_EmissionColor");
        highlightingStripsParent.transform.GetChild(index).gameObject.SetActive(true);
        StartCoroutine(HideHighlights(highlightingStripsParent.transform.GetChild(index).gameObject));
    }
    IEnumerator HideHighlights(GameObject strip)
    {
        yield return new WaitForSeconds(0.5f);
        strip.SetActive(false);
    }

    public int GetMaxBlock()
    {
        int maxBlock = 0;
        int min = int.MinValue;
        for(int i = 0; i < BlockParent.transform.childCount; i++)
        {
            string val = BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
            if(int.Parse(val) > min)
            {
                min = int.Parse(val);
            }
        }
        maxBlock = min;
        return maxBlock;
    }
    int NextBlockValue = 2;
    public TMP_Text NextBlockTextValue;

    void UpdateNextBlockUI()
    {
        NextBlockTextValue.text = Mathf.Pow(2, NextBlockValue).ToString();
    }
    public void SpawnBlock()
    {
        if(!isGameOver)
        {
            int randomNum = Random.Range(0, 5);
            GameObject newBlock = Instantiate(mergeBlockPrefab, spawnPointsParent.transform.GetChild(randomNum).GetChild(0).transform.position, Quaternion.identity);
            isSlided = false;
            fallingBlock = newBlock;
            float target = 2;
            int power = 1;
            if(count < 10)
            {
                power = Random.Range(1, 6);
            }
            else
            {
                if(BlockParent.transform.childCount < 10)
                {
                    int num1 = GetMaxBlock();
                    int highestPower = (int)Mathf.Log(num1, 2);
                    power = Random.Range(1, highestPower-highestPower/2);
                }
                else
                {
                    List<int> values = new List<int>();
                    List<float> xposes = new List<float>();
                    for(int i = 0; i < BlockParent.transform.childCount;i++)
                    {
                        if(!xposes.Contains(BlockParent.transform.GetChild(i).transform.position.x))
                        {
                            xposes.Add(BlockParent.transform.GetChild(i).transform.position.x);
                        }
                    }

                    for(int i = 0; i < xposes.Count;i++)
                    {
                        int tempVal = 2;
                        float ypos = float.MinValue;
                        for(int j = 0; j < BlockParent.transform.childCount;j++)
                        {
                            if(BlockParent.transform.GetChild(j).transform.position.x == xposes[i])
                            {
                                if(BlockParent.transform.GetChild(j).transform.position.y >= ypos)
                                {
                                    ypos = BlockParent.transform.GetChild(j).transform.position.y;
                                    tempVal = int.Parse(BlockParent.transform.GetChild(j).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
                                }
                            }
                        }
                        values.Add(tempVal);
                    }
                    power = (int)Mathf.Log(values[Random.Range(0, values.Count)], 2);
                }
                
            }
            target = Mathf.Pow(target, NextBlockValue);
            NextBlockValue = power;
            UpdateNextBlockUI();
            PlayerPrefs.SetFloat("currentValue", target);
            newBlock.transform.parent = BlockParent.transform;
            BlockParent.transform.GetChild(BlockParent.transform.childCount - 1).transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = target.ToString();
            float finalVal = target;
            if (finalVal >= 100)
            {
                newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 35;
            }
            else if (finalVal >= 1000 && finalVal < 10000)
            {
                newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 30;
            }
            else if (finalVal >= 10000 && finalVal < 100000)
            {
                newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 25;
            }
            else if (finalVal >= 100000 && finalVal < 1000000)
            {
                newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 20;
            }
            else if (finalVal >= 1000000)
            {
                newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 18;
            }
            newBlock.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            changeColorAccordingly(target, newBlock);
            count++;
        }
        
    }

    void changeTextSize(int finalVal, GameObject newBlock)
    {
        if (finalVal >= 100)
        {
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 35;
        }
        else if (finalVal >= 1000 && finalVal < 10000)
        {
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 30;
        }
        else if (finalVal >= 10000 && finalVal < 100000)
        {
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 25;
        }
        else if (finalVal >= 100000 && finalVal < 1000000)
        {
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 20;
        }
        else if (finalVal >= 1000000)
        {
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontSize = 18;
        }
    }
}