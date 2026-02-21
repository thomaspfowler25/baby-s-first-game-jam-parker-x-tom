using System.Collections;
using TMPro;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Robots")]
    public RobotController player1Robot;
    public RobotController player2Robot;

    [Header("Spawns")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Rules")]
    public int roundsToWin = 3;
    public float timeBetweenRounds = 1.0f;

    [Header("UI (TextMeshProUGUI)")]
    public TextMeshProUGUI player1StatusText;
    public TextMeshProUGUI player2StatusText;
    public TextMeshProUGUI centerMessageText;

    private RobotHealth _p1Health;
    private RobotHealth _p2Health;

    private int _p1RoundsWon = 0;
    private int _p2RoundsWon = 0;

    private bool _roundInProgress = false;
    private bool _matchOver = false;

    private void Awake()
    {
        if (player1Robot == null || player2Robot == null)
        {
            Debug.LogError("MatchManager: Assign both player robots in the Inspector.");
            enabled = false;
            return;
        }

        _p1Health = player1Robot.GetComponent<RobotHealth>();
        _p2Health = player2Robot.GetComponent<RobotHealth>();

        if (_p1Health == null || _p2Health == null)
        {
            Debug.LogError("MatchManager: Each robot must have a RobotHealth component.");
            enabled = false;
            return;
        }

        _p1Health.OnDied += HandleRobotDied;
        _p2Health.OnDied += HandleRobotDied;

        _p1Health.OnHealthChanged += _ => UpdateHUD();
        _p2Health.OnHealthChanged += _ => UpdateHUD();
    }

    private void Start()
    {
        UpdateHUD();
        StartCoroutine(BeginRoundRoutine());
    }

    private void Update()
    {
        if (_matchOver && Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator BeginRoundRoutine()
    {
        _roundInProgress = false;

        ResetRobotsForNewRound();

        player1Robot.SetInputsEnabled(false);
        player2Robot.SetInputsEnabled(false);

        yield return StartCoroutine(CountdownRoutine());

        player1Robot.SetInputsEnabled(true);
        player2Robot.SetInputsEnabled(true);

        _roundInProgress = true;
        if (centerMessageText != null) centerMessageText.text = "";
    }

    private void ResetRobotsForNewRound()
    {
        if (player1Spawn != null) player1Robot.HardResetPose(player1Spawn.position);
        if (player2Spawn != null) player2Robot.HardResetPose(player2Spawn.position);

        _p1Health.ResetToFull();
        _p2Health.ResetToFull();
        UpdateHUD();
    }

    private IEnumerator CountdownRoutine()
    {
        if (centerMessageText == null) yield break;

        centerMessageText.text = "3";
        yield return new WaitForSeconds(1f);

        centerMessageText.text = "2";
        yield return new WaitForSeconds(1f);

        centerMessageText.text = "1";
        yield return new WaitForSeconds(1f);

        centerMessageText.text = "GO!";
        yield return new WaitForSeconds(0.6f);
    }

    private void HandleRobotDied(RobotHealth deadHealth)
    {
        if (!_roundInProgress || _matchOver) return;

        _roundInProgress = false;

        player1Robot.SetInputsEnabled(false);
        player2Robot.SetInputsEnabled(false);

        if (deadHealth == _p1Health)
        {
            _p2RoundsWon++;
            StartCoroutine(EndRoundRoutine("Player 2 wins the round!"));
        }
        else if (deadHealth == _p2Health)
        {
            _p1RoundsWon++;
            StartCoroutine(EndRoundRoutine("Player 1 wins the round!"));
        }

        UpdateHUD();
    }

    private IEnumerator EndRoundRoutine(string message)
    {
        if (centerMessageText != null)
            centerMessageText.text = message;

        if (_p1RoundsWon >= roundsToWin || _p2RoundsWon >= roundsToWin)
        {
            _matchOver = true;

            string winner = (_p1RoundsWon > _p2RoundsWon) ? "PLAYER 1" : "PLAYER 2";
            if (centerMessageText != null)
                centerMessageText.text = $"{winner} WINS THE MATCH!\nPress R to Restart";

            yield break;
        }

        yield return new WaitForSeconds(timeBetweenRounds);
        StartCoroutine(BeginRoundRoutine());
    }

    private void UpdateHUD()
    {
        if (player1StatusText != null)
        {
            player1StatusText.text =
                $"P1 ({player1Robot.BodyType})  HP: {_p1Health.CurrentHearts:0.0}/{_p1Health.MaxHearts:0.0}  Rounds: {_p1RoundsWon}/{roundsToWin}";
        }

        if (player2StatusText != null)
        {
            player2StatusText.text =
                $"P2 ({player2Robot.BodyType})  HP: {_p2Health.CurrentHearts:0.0}/{_p2Health.MaxHearts:0.0}  Rounds: {_p2RoundsWon}/{roundsToWin}";
        }
    }
}
