using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MENU_STATE { INICIO, MENU, SALIDA };

public class GameController : MonoBehaviour
{
    public static GameController GC;
    public GAME_STATE state;
    public Transform Player;

    private bool loadComponent;
    private string sceneName;

    //transition
    Animator transitionFadeAnim;

    //HUD
    Slider health;

    // player
    // => health
    bool cooldown = false;
    float timeCooldown = 0.0f;
    public GameObject dust;

    void Awake()
    {
        if (GC == null)
        {
            GC = this;
            DontDestroyOnLoad(this.gameObject);

            //state = GAME_STATE.M_INICIO;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        //state = GAME_STATE.JUGANDO; //Cambiar a INICIO una vez que se incluya algun tipo de introduccion idk
    }

    void Update()
    {
        /* SOLO PARA DESARROLLO */
        if (!loadComponent)
        {
            sceneName = SceneManager.GetActiveScene().name;
            
            transitionFadeAnim = GameObject.FindGameObjectWithTag("Transition/Fade").GetComponent<Animator>();

            switch (sceneName)
            {
                case "Menu":
                    state = GAME_STATE.MENU_INICIO;
                    break;
                case "ShootingScene":
                    state = GAME_STATE.INICIO;
                    Player = GameObject.FindGameObjectWithTag("Player").transform;
                    break;
                case "TestScene":
                    state = GAME_STATE.INICIO;
                    health = GameObject.FindGameObjectWithTag("Player/Health").GetComponent<Slider>();
                    Debug.Log(health.value);
                    Player = GameObject.FindGameObjectWithTag("Player").transform;
                    Player.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            loadComponent = true;
        }
        /* SOLO PARA DESARROLLO */

        /*STATE MACHINE*/
        switch (state)
        {
            case GAME_STATE.MENU_INICIO:
                if (!AnimationActive(transitionFadeAnim, 0, "FadeIn"))
                {
                    state = GAME_STATE.MENU;
                }
                break;
            case GAME_STATE.MENU:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = GAME_STATE.MENU_SALIDA;
                    transitionFadeAnim.SetTrigger("out");
                }
                break;
            case GAME_STATE.MENU_SALIDA:
                if (!AnimationActive(transitionFadeAnim, 0, "FadeOut"))
                {
                    ChangeScene("TestScene", false);
                    state = GAME_STATE.INICIO;
                }
                break;
            case GAME_STATE.INICIO:
                if (!AnimationActive(transitionFadeAnim, 0, "FadeIn"))
                {
                    state = GAME_STATE.JUGANDO;
                }
                break;
            case GAME_STATE.JUGANDO:
                break;
            case GAME_STATE.PAUSA:
                break;
            case GAME_STATE.REINICIO:
                if (!AnimationActive(transitionFadeAnim, 0, "FadeOut"))
                {
                    Debug.Log("entro");
                    state = GAME_STATE.INICIO;
                    ChangeScene("TestScene", false);
                }
                break;
            case GAME_STATE.FIN:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = GAME_STATE.REINICIO;
                    transitionFadeAnim.SetTrigger("out");
                }
                break;
            default:
                break;
        }
    }

    void ChangeScene(string sceneName, bool load)
    {
        SceneManager.LoadScene(sceneName);
        loadComponent = load;
    }

    bool AnimationActive(Animator anim, int layer, string name)
    {
        return anim.GetCurrentAnimatorStateInfo(layer).IsName(name);
    }

    // player
    // => damage
    public void TakeDamage(Transform obj, int damage, float time)
    {
        Dust(obj);

        if (!cooldown)
        {
            if (health.value > 0)
            {
                health.value -= damage;
                timeCooldown = time;
                cooldown = true;

                StartCoroutine(ResetCooldown());
            }
            else if (health.value <= 0)
            {
                Player.gameObject.SetActive(false);
                state = GAME_STATE.FIN;
            }
        }
    }

    // => health
    public void TakeHealth(int life)
    {
        health.value += life;
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(timeCooldown);
        cooldown = false;
    }

    void Dust(Transform obj)
    {
        GameObject temp = Instantiate(dust, obj.position, obj.rotation);
        Destroy(temp, 1.5f);
    }
}

public enum GAME_STATE { /*MENU*/MENU_INICIO, MENU, MENU_SALIDA, /*GAME*/ INICIO, JUGANDO, PAUSA, REINICIO, FIN }
public enum PLAYER_STATE { CONTROLANDO, INACTIVO, MUERTO }
