using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StickHeroController : MonoBehaviour
{
    public GameObject hero;
    public GameObject stick;
    public GameObject platformPrefab;
    public GameObject perfectZonePrefab;
    public Text scoreText;
    public Button restartButton;

    private string phase = "waiting";
    private float stickStretchSpeed = 5f;
    private float heroWalkSpeed = 2f;
    private float cameraTransitionSpeed = 2f;

    private int score = 0;
    private Transform currentPlatform;
    private Transform nextPlatform;
    private Transform perfectZone;

    void Start()
    {
        restartButton.gameObject.SetActive(false);
        GenerateInitialPlatforms();
        UpdateScoreText();
    }

    void Update()
    {
        switch (phase)
        {
            case "waiting":
                if (Input.GetMouseButtonDown(0))
                {
                    phase = "stretching";
                }
                break;

            case "stretching":
                StretchStick();
                if (Input.GetMouseButtonUp(0))
                {
                    phase = "turning";
                }
                break;

            case "turning":
                RotateStick();
                break;

            case "walking":
                WalkHero();
                break;

            case "falling":
                HeroFall();
                break;

            case "transitioning":
                TransitionCamera();
                break;
        }
    }

    private void StretchStick()
    {
        stick.transform.localScale += new Vector3(0, stickStretchSpeed * Time.deltaTime, 0);
    }

    private void RotateStick()
    {
        stick.transform.Rotate(0, 0, -90 * Time.deltaTime);

        if (stick.transform.eulerAngles.z <= 270) // Stick is horizontal
        {
            stick.transform.eulerAngles = new Vector3(0, 0, 270);
            CheckStickHit();
        }
    }

    private void CheckStickHit()
    {
        float stickEnd = stick.transform.position.x + stick.transform.localScale.y;
        float platformStart = nextPlatform.position.x - nextPlatform.localScale.x / 2;
        float platformEnd = nextPlatform.position.x + nextPlatform.localScale.x / 2;

        if (stickEnd >= platformStart && stickEnd <= platformEnd)
        {
            float perfectZoneStart = perfectZone.position.x - perfectZone.localScale.x / 2;
            float perfectZoneEnd = perfectZone.position.x + perfectZone.localScale.x / 2;

            if (stickEnd >= perfectZoneStart && stickEnd <= perfectZoneEnd)
            {
                score += 2; // Perfect hit
            }
            else
            {
                score += 1;
            }

            UpdateScoreText();
            phase = "walking";
        }
        else
        {
            phase = "falling";
        }
    }

    private void WalkHero()
    {
        hero.transform.position += Vector3.right * heroWalkSpeed * Time.deltaTime;

        if (hero.transform.position.x >= nextPlatform.position.x - nextPlatform.localScale.x / 2)
        {
            phase = "transitioning";
        }
    }

    private void HeroFall()
    {
        hero.transform.position += Vector3.down * heroWalkSpeed * Time.deltaTime;

        if (hero.transform.position.y < -5) // Fall off-screen
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    private void TransitionCamera()
    {
        Vector3 targetPosition = new Vector3(nextPlatform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, targetPosition) < 0.01f)
        {
            GenerateNextPlatform();
            phase = "waiting";
        }
    }

    private void GenerateInitialPlatforms()
    {
        currentPlatform = Instantiate(platformPrefab, new Vector3(-3, -3, 0), Quaternion.identity).transform;
        nextPlatform = Instantiate(platformPrefab, new Vector3(5, -3, 0), Quaternion.identity).transform;
        perfectZone = Instantiate(perfectZonePrefab, nextPlatform.position, Quaternion.identity).transform;
    }

    private void GenerateNextPlatform()
    {
        Destroy(currentPlatform.gameObject);

        currentPlatform = nextPlatform;
        nextPlatform = Instantiate(platformPrefab, new Vector3(Random.Range(8, 12), -3, 0), Quaternion.identity).transform;
        perfectZone = Instantiate(perfectZonePrefab, nextPlatform.position, Quaternion.identity).transform;

        // Reset stick
        stick.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        stick.transform.eulerAngles = Vector3.zero;
        stick.transform.position = hero.transform.position + Vector3.up * 0.1f;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}