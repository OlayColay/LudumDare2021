using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattle : MonoBehaviour
{
    /// <summary> The opponents that the player will fight in the battle </summary>
    public GameObject[] opponents;

    public AudioSource currentMusic;

    private Collider2D player;

    private bool battledOnce = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Start Battle");
        if (other.tag == "Player")
        {
            currentMusic.Pause();
            GameObject.FindObjectOfType<OverworldMovement>().controls.Overworld.Disable();
            SceneManager.LoadScene("Battle", LoadSceneMode.Additive);

            if (!battledOnce)
            {
                SceneManager.sceneLoaded += BattleLoaded;
                player = other;
                battledOnce = true;
            }
        }
    }

    // This goes before any Start function in the scene!
    private void BattleLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GetComponent<Collider2D>().IsTouching(player))
        {
            GameObject.FindObjectOfType<BattleManager>().SpawnOpponents(opponents, this.gameObject);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = false;
        }
    }
}
