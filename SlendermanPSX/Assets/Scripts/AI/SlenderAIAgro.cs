using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlenderAIAgro : MonoBehaviour
{
    public Rigidbody playerRb;
    public NavMeshAgent ai;

    public SkinnedMeshRenderer slenderMesh;

    public float m_speed, staticIncreaseRate, staticDecreaseRate, soundIncreaseRate, soundDecreaseRate, healthIncreaseRate, healthDecreaseRate, catchDistance;

    public float playerHealth = 100;

    public Slider healthSlider;
    public Image sliderFill, medical;


    public GameObject player;

    public Transform slenderMainTransform, playerTransform;

    public GameObject jumpscareCam, blackscreen, tryAgainText, whiteScreen, succeedText;

    Vector3 dest;

    int randNum, randNum2;

    int teleportChance; 

    int token, token3, token4;

    public bool usingHealthSlider, enableCursorAfterDeath;
    
    public string scenename;

    float aiDistance, staticAmount, staticVolume;

    public List<Transform> teleportDestinations;

    public raycastSlender raycastScript;

    public RawImage staticscreen;

    public Color staticOpacity;

    public AudioSource staticSound, jumpscareSound;

    public Camera playerCam;
    public GameObject slender2;
    public Animator slenderAnim;
    public bool attackRunning = false;
    public Target target;
    public ParticleSystem deathExplosion;
    public AudioSource endSound, endTalk, bossMusic;

    void Start()
    {
        AudioListener.pause = false; 
        bossMusic.gameObject.SetActive(true);
        // playerHealth = player.GetComponent<FPSController>().playerHealth;
        // player.SetActive(true);

    }

    void resetSlender()
    {
        teleportChance = Random.Range(0, 3);
        if (teleportChance != 0)
        {
            randNum = Random.Range(0, teleportDestinations.Count);
            slenderMainTransform.position = teleportDestinations[randNum].position;
        }
    }

    IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(2.5f);
        blackscreen.SetActive(true);
        tryAgainText.SetActive(true);

        // AudioListener.pause = true;
        yield return new WaitForSeconds(2f);
        if (enableCursorAfterDeath == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        SceneManager.LoadScene(scenename);
    }

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);

        // if (GeometryUtility.TestPlanesAABB(planes, slenderMesh.bounds) && playerHealth > 0)
        // {
        //     ai.enabled = false;
        //     ai.speed = 0;
        //     slenderAnim.ResetTrigger("walking");
        //     slenderAnim.SetTrigger("idle");

        //     ai.SetDestination(transform.position);
        //     if (raycastScript.detected == true)
        //     {
        //         token3 = 0;
        //         if (token4 == 0)
        //         {
        //             randNum2 = Random.Range(0, 2);
        //             if (randNum2 == 0)
        //             {
        //                 jumpscareSound.Play();
        //             }
        //             token4 = 1;
        //         }
        //         slenderMainTransform.position = slenderMainTransform.position;
        //         staticVolume += soundIncreaseRate * Time.deltaTime;
        //         staticAmount += staticIncreaseRate * Time.deltaTime;
        //         playerHealth -= healthDecreaseRate * Time.deltaTime;
        //         if (staticVolume > 1)
        //         {
        //             staticVolume = 1;
        //         }
        //         if (staticAmount > 0.9f)
        //         {
        //             staticAmount = 0.9f;
        //         }
        //     }
        // }

        // if (!GeometryUtility.TestPlanesAABB(planes, slenderMesh.bounds) && playerHealth > 0 || raycastScript.detected == false && playerHealth > 0)
        // {
            if (aiDistance > catchDistance)
                ai.speed = m_speed;
            ai.enabled = true;
            slenderAnim.ResetTrigger("idle");
            slenderAnim.SetTrigger("moving");

            dest = playerTransform.position;
            ai.destination = dest;
            if (playerHealth > 100)
            {
                playerHealth = 100;
            }
        // }

        if (usingHealthSlider == true)
        {
            healthSlider.value = playerHealth;
            sliderFill.color = Color.Lerp(Color.red, Color.green, playerHealth / 100);
            medical.color = Color.Lerp(Color.red, Color.green, playerHealth / 100);
        }
        staticSound.volume = staticVolume;
        staticOpacity.a = staticAmount;
        staticscreen.color = staticOpacity;

        transform.LookAt(new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z));

        aiDistance = Vector3.Distance(this.transform.position, playerTransform.position);
        // Debug.Log(aiDistance);

        if (playerHealth <= 0)
        {
            StartCoroutine(KillPlayer());
            staticVolume += soundIncreaseRate * Time.deltaTime;
            staticAmount += staticIncreaseRate * Time.deltaTime;
            if (staticVolume > 1)
            {
                staticVolume = 1;
            }
            if (staticAmount > 0.9f)
            {
                staticAmount = 0.9f;
            }
            playerTransform.gameObject.SetActive(false);
            jumpscareCam.SetActive(true);
            ai.speed = 0;
            ai.SetDestination(transform.position);
        }
        else if (aiDistance <= catchDistance)
        {
            if(!attackRunning){
                if(Random.Range(0, 2) == 0){
                    StartCoroutine(AttackPlayer0());
                }
                else
                {
                    StartCoroutine(AttackPlayer1());
                }
            }


            // slenderAnim.Play("Attack 0");
            // ai.speed = 0;
            // new WaitForSeconds(3.5f);
            // playerHealth -= 1;
            // slenderAnim.Play("Run");
            // if (token == 0)
            // {
            //     playerHealth = 0;
            //     token = 1;
            // }
        }
    }
    IEnumerator AttackPlayer1(){
        attackRunning = true;
        ai.enabled = false;
        // ai.speed = 0;
        slenderAnim.Play("Attack 1");
        jumpscareSound.Play();
        yield return new WaitForSeconds(2.5f);
        if (Vector3.Distance(transform.position, playerTransform.position) <= catchDistance)
        {
            playerHealth -= 15;
            // playerTransform.position -= playerTransform.forward * 0.1f;
            // playerTransform.position += playerTransform.up * 0.5f;
            // playerRb.AddForce(-playerTransform.forward * 1000);
            // playerRb.AddForce(playerTransform.up * 1000);

        }
        ai.enabled = true;
        ai.speed = m_speed;
        attackRunning = false;
    }
    IEnumerator AttackPlayer0()
    {
        attackRunning = true;
        ai.enabled = false;
        // ai.speed = 0;
        slenderAnim.Play("Attack 0");
        yield return new WaitForSeconds(2f);
        if (Vector3.Distance(transform.position, playerTransform.position) <= catchDistance)
        {
            playerHealth -= 20;
            target.Knockback(-player.transform.forward, 0.3f);
            target.Knockback(player.transform.up, 2f);
            // playerTransform.position -= playerTransform.forward * 0.1f;
            // playerTransform.position += playerTransform.up * 0.5f;
            // playerRb.AddForce(-playerTransform.forward * 1000);
            // playerRb.AddForce(playerTransform.up * 1000);
            // player.transform.position -= player.transform.forward * 0.1f;


        }
        ai.enabled = true;
        ai.speed = m_speed;
        attackRunning = false;
    }

    internal void Die()
    {
        bossMusic.gameObject.SetActive(false);

        // ai.enabled = false;
        // ai.speed = 0;
        // catchDistance = -1;
        // slenderMesh.enabled = false;
        // slenderMesh.gameObject.GetComponent<Collider>().enabled = false;
        gameObject.transform.position = new Vector3(-1000, -1000, -1000);
        
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        // gameObject.SetActive(false);
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        
        endSound.Play();
        endSound.volume = Mathf.Lerp(0, 1, 2f);

        endTalk.Play();

        whiteScreen.SetActive(true);
        // while (whiteScreen.GetComponent<Image>().color.a < 1)
        // {
        //     whiteScreen.GetComponent<Image>().color = Color.Lerp(whiteScreen.GetComponent<Image>().color, new Color(1, 1, 1, 1), 4f * Time.deltaTime);
        //     yield return null;
        // }
        // whiteScreen.GetComponent<Animator>().SetTrigger("FadeIn");
        whiteScreen.GetComponent<Image>().color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 4f);

        succeedText.SetActive(true);
        // while (succeedText.GetComponent<Image>().color.a < 1)
        // {
        //     succeedText.GetComponent<Image>().color = Color.Lerp(succeedText.GetComponent<Image>().color, new Color(1, 1, 1, 1), 4f * Time.deltaTime);
        //     yield return null;
        // }
        // succeedText.GetComponent<Animator>().SetTrigger("FadeIn");
        // succeedText.GetComponent<Image>().color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 4f);

        yield return new WaitForSeconds(4f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu");
    }
}
