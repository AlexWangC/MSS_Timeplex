using System;
using System.Collections;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

// this script handles the movement & movement inheritance of all players
public class scrMoveInheritanceManager : MonoBehaviour
{
    // for everything to work well, make sure to tune the delay well!! Move Delay should be larger than first enemy move delay + enemy move delay.
    public float Move_delay;
    public float First_enemy_move_delay;
    public float Enemies_move_delay;
    
    [HideInInspector] public bool Can_move;
    [HideInInspector] public scrPlayer[] Players;

    private bool is_moving = false;

    private void Start()
    {
        Can_move = true;
        // get all players from the scene.
        ObtainPlayers();
        checkPlayersContent();
        Players = sortPlayerByPanelTimeIndex(Players);
        checkPlayersContent();
    }
    
    /*
     * Below is the movement grand switch
     */
    private void Update()
    {
        if (is_moving) return;
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            ObtainPlayers();
            Players = sortPlayerByPanelTimeIndex(Players);
            
            //while it is in delay, let's disable the panel sprite for highlight.
            
            StartCoroutine(movePlayerDelayed(0, Move_delay));
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ObtainPlayers();
            Players = sortPlayerByPanelTimeIndex(Players);
            
            StartCoroutine(movePlayerDelayed(1, Move_delay));
            
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ObtainPlayers();
            Players = sortPlayerByPanelTimeIndex(Players);
            
            StartCoroutine(movePlayerDelayed(2, Move_delay));
            
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ObtainPlayers();
            Players = sortPlayerByPanelTimeIndex(Players);
            
            StartCoroutine(movePlayerDelayed(3, Move_delay));
            
        }

        
    }
    /*
     * Above is the movement grand switch
     */

    public scrPlayer[] ObtainPlayers() // updates & gets players in an array
    {
        Players = FindObjectsByType<scrPlayer>(FindObjectsSortMode.None);
        return Players;
    }

    private scrPlayer[] sortPlayerByPanelTimeIndex(scrPlayer[] players) // increasing order using bubble sort.
    {
        for (int i = 0; i < players.Length - 1; i++)
        {
            for (int j = 0; j < players.Length - 1 - i; j++)
            {
                if (players[j].GetComponentInParent<scrPanel>().Time_index >
                    players[j + 1].GetComponentInParent<scrPanel>().Time_index)
                {
                    scrPlayer temp = players[j];
                    players[j] = players[j + 1];
                    players[j + 1] = temp;
                }
            }
        }
        
        
        // if the neighboring player at front is older, swap back
        for (int k = 0; k < players.Length - 1; k++)
        {
            for (int l = 0; l < players.Length - 1 - k; l++)
            {
                //if player[l] has a bigger time index than [l+1] AND they have the same time index
                if ((players[l].GetComponent<scrPlayer>().playerAgeIndex >
                    players[l + 1].GetComponent<scrPlayer>().playerAgeIndex) && players[l].GetComponentInParent<scrPanel>().Time_index ==
                    players[l + 1].GetComponentInParent<scrPanel>().Time_index)
                {
                    scrPlayer temp = players[l];
                    players[l] = players[l + 1];
                    players[l + 1] = temp;
                }
            }
        }
        
        

        return players;
    }

    private void checkPlayersContent() // just for debugging
    {
        if (Players != null && Players.Length > 0)
        {
            Debug.Log("Finding Players... \nNumber of Players Found: " + Players.Length);
            for (int i = 0; i < Players.Length; i++)
            {
                Debug.Log($"Player {i}: {Players[i].name}");
            }
        }
        else
        {
            Debug.Log("No players found!");
        }
    }

    private bool movePlayer(int player_index, int direction)
    {
        scrPlayer player = Players[player_index];
        return player.Move(direction);
    }

    //how should we do it?
    //move all
    IEnumerator movePlayerDelayed(int direction, float delay)
    {
        is_moving = true;
        
        int current_player = 0;
        while (current_player < Players.Length)
        {
            if (Players[current_player] != null)
            { 
                Players[current_player].Move(direction);
                highlightPanelBeforeMoving(Players[current_player]);
                yield return new WaitForSeconds(delay);
                delightPanelAfterMoving(Players[current_player]);
                current_player++;
            }
        }

        is_moving = false;
        Can_move = true;
    }

    private void highlightPanelBeforeMoving(scrPlayer player)
    {
        if (player != null && player.GetComponentInParent<scrPanel>().Dead == false )
        {
            player.GetComponentInParent<scrPanel>().GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void delightPanelAfterMoving(scrPlayer player)
    {
        if (player != null)
        {
            player.GetComponentInParent<scrPanel>().GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
