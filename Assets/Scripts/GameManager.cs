using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Home[] homes;
    [SerializeField] private Frogger frogger;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Text timeText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text scoreText;

    private int lives;
    private int score;
    private int time;

    public int Lives => lives;
    public int Score => score;
    public int Time => time;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        NewGame();
    }

    //Empezar Fresh de 0
    private void NewGame()
    {
        gameOverMenu.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewLevel();
    }

    //Cuando ocupas las 5 casas invocas a new level
    public void NewLevel()
    {
        //empiezas un nuevo nivel entonces las casas no estan ocupadas, aqui las seteamos
        for (int i = 0; i < homes.Length; i++)
        {
            homes[i].enabled = false;
        }

        Respawn();
    }

    private void Respawn()
    {
        frogger.Respawn();

        StopAllCoroutines();
        StartCoroutine(Timer(30));
    }

    private IEnumerator Timer(int duration)
    {
        time = duration;
        timeText.text = time.ToString();
        //mientras tengamos tiempo entonces esperaremos un segundo y reduciremos el tiempo de 1 en 1
        while (time > 0)
        {
            yield return new WaitForSeconds(1);

            time--;
            timeText.text = time.ToString();
        }
        // si no lo hiciste a tiempo entonces mueres
        frogger.Die();
    }

    public void Died()
    {
        //eliminamos una vida
        SetLives(lives - 1);
        //chequeamos si tenemos mas vidas
        if (lives > 0)
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        {
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        //ocultamos a frogger
        frogger.gameObject.SetActive(false);
        //mostramos un menu de game over
        gameOverMenu.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    }

    private IEnumerator CheckForPlayAgain()
    {
        //por defecto el jugador no quiere jugar otra vez
        bool playAgain = false;


        while (!playAgain)
        {
            //si el juegador presiona return seteamos play again a true y saldra de este while
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playAgain = true;
            }

            yield return null;
        }
        //invocamos a new game si sale del loop
        NewGame();
    }

    //Si avanzamos una fila entonces incrementamos la puntuacion
    public void AdvancedRow()
    {
        SetScore(score + 10);
    }

    public void HomeOccupied()
    {
        //seteamos que frogger se apague o no este activo temporalmente
        frogger.gameObject.SetActive(false);

        //tienes bonus si ocupas las casas mas rapido
        int bonusPoints = time * 20;
        //si ocupamos una casa subimos el score
        SetScore(score + bonusPoints + 50);
        //si superaste el nivel entonces emepezaremos un nuevo nivel
        if (Cleared())
        {
            //si ganamos el nivel le subimos una vida
            SetLives(lives + 1);
            //si ganamos el nivel recibimos mas puntos
            SetScore(score + 1000);
            //invocamos el nuevo nivel esperando 1s
            Invoke(nameof(NewLevel), 1f);
        }
        else
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    //true si superamos el nivel
    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            //si no esta disponible entonces todavia no superaste el nivel
            if (!homes[i].enabled)
            {
                return false;
            }
        }

        return true;
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }

}
