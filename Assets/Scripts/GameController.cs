using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    public bool freshStart = false;
    public Animator swordAnim;
    private string failAnimName = "SwordStone_Pull";
    private string successAnimName = "SwordStone_Victory";
    private string idleAnimName = "SwordStone_Bob";

    public SpriteRenderer indicator;
    public List<Sprite> directionalSprites; // 0: up, 1: left, 2: down, 3: right

    private int attempts = 0;
    public ShadowText attemptsText;
    private CanvasGroup attemptsCanvasGroup;

    public Image backgroundImage;

    private int currentDirection = -1; // 0: up, 1: left, 2: down, 3: right
    private int previousDirection = -1;
    private bool inputAllowed = false;
    private bool gameEnded = false;

    public GameObject victoryScreen;

    void Start()
    {
        if (freshStart)
        {
            PlayerPrefs.DeleteAll();
        }
        else
        {
            if (PlayerPrefs.HasKey("Attempts"))
            {
                attempts = PlayerPrefs.GetInt("Attempts");
            }
            if (PlayerPrefs.HasKey("Victory"))
            {
                swordAnim.Play(idleAnimName);
                // Fade out attemptsText if victory already achieved
                attemptsCanvasGroup = attemptsText.GetComponent<CanvasGroup>();
                if (attemptsCanvasGroup != null)
                {
                    attemptsCanvasGroup.alpha = 0f;
                }
                else
                {
                    attemptsText.gameObject.SetActive(false);
                }

                if (!PlayerPrefs.HasKey("Name"))
                {
                    victoryScreen.SetActive(true);
                }

                return;
            }
        }

        attemptsText.SetText($"Attempts: {attempts}");
        attemptsCanvasGroup = attemptsText.GetComponent<CanvasGroup>();

        StartNewTurn();
    }

    void Update()
    {
        if (!inputAllowed || gameEnded) return;

        // Only accept input if no other direction keys are held
        if (IsSingleDirectionKeyPressed(out int pressedDir))
        {
            if (pressedDir == currentDirection)
            {
                inputAllowed = false;
                indicator.sprite = null;
                CheckSuccess();
            }
        }
    }

    void StartNewTurn()
    {
        int newDirection;
        do
        {
            newDirection = Random.Range(0, 4);
        } while (newDirection == previousDirection);
        currentDirection = newDirection;
        previousDirection = currentDirection;
        indicator.sprite = directionalSprites[currentDirection];
        inputAllowed = true;
    }

    bool IsSingleDirectionKeyPressed(out int dir)
    {
        dir = -1;
        bool[] keys = new bool[4];
        // Up: W or UpArrow
        keys[0] = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        // Left: A or LeftArrow
        keys[1] = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        // Down: S or DownArrow
        keys[2] = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        // Right: D or RightArrow
        keys[3] = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (keys[i])
            {
                dir = i;
                count++;
            }
        }
        return count == 1;
    }

    void CheckSuccess()
    {
        attempts++;
        attemptsText.SetText($"Attempts: {attempts}");

        // 1 in 1,000,000 chance
        if (Random.Range(0, 1000000) == 0)
        {
            gameEnded = true;
            PlayerPrefs.SetInt("Victory", 1);
            StartCoroutine(DoSuccess());
        }
        else
        {
            PlayerPrefs.SetInt("Attempts", attempts);
            swordAnim.Play(failAnimName);
            StartCoroutine(WaitForFailAnim());
        }
    }

    System.Collections.IEnumerator WaitForFailAnim()
    {
        // Wait for fail animation to finish
        yield return new WaitForSeconds(0.25f);
        StartNewTurn();
    }

    IEnumerator DoSuccess()
    {
        swordAnim.Play(successAnimName);

        // Start fading out attemptsText
        if (attemptsCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroupAlpha(attemptsCanvasGroup, 1f, 0f, 1f));
        }
        else
        {
            attemptsText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(6f);

        victoryScreen.SetActive(true);
    }

    IEnumerator FadeCanvasGroupAlpha(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}
