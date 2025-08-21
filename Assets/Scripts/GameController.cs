using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public bool freshStart = false;
    public Animator swordAnim;
    private string failAnimName = "SwordStone_Pull";
    private string successAnimName = "SwordStone_Victory";

    public SpriteRenderer indicator;
    public List<Sprite> directionalSprites; // 0: up, 1: left, 2: down, 3: right

    private int attempts = 0;
    public ShadowText attemptsText;

    public Image backgroundImage;

    private int currentDirection = -1; // 0: up, 1: left, 2: down, 3: right
    private int previousDirection = -1;
    private bool inputAllowed = false;
    private bool gameEnded = false;

    void Start()
    {
        if (freshStart)
        {
            PlayerPrefs.DeleteAll();
        }
        else if (PlayerPrefs.HasKey("Attempts"))
        {
            attempts = PlayerPrefs.GetInt("Attempts");
        }
        attemptsText.SetText($"Attempts: {attempts}");

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
        // 1 in 1,000,000 chance
        if (Random.Range(0, 1000000) == 0)
        {
            swordAnim.Play(successAnimName);
            gameEnded = true;
        }
        else
        {
            attempts++;
            if (attemptsText != null)
                attemptsText.SetText($"Attempts: {attempts}");
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
}
