using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class scrGoalManager : MonoBehaviour
{
    //public bool LockAllGoals; // whether you want to have all goals be locked in the scene. NOT YET IMPLEMENTED
    private scrGoal[] allGoals;

    private void Start()
    {
        initializeGoals();
        //Debug.Log("Next Scene Index = " + (SceneManager.GetActiveScene().buildIndex + 1));
        //lockingAllGoals();
    }


    /*
    public void GoalsReached() //subscribe this to an event maybe, or call it from somewhere else
    {
        Debug.Log("Goals Reached check invoked...");
        if (checkIfAllPlayerAtSameDoor())
        {
            //play sound
            scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.level_clear, this.transform, 1);
            
            Debug.Log("Hey all goals have been reached");
            // content here
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene
        }
    }
    */

    private void initializeGoals()
    {
        allGoals = FindObjectsByType<scrGoal>(FindObjectsSortMode.None);
    }


    public void LoadScene(String nextSceneName)
    {
        print("Load Scene");
        print(checkIfAllPlayerAtSameDoor());

        if (!checkIfAllPlayerAtSameDoor()) return;
        
        SceneManager.LoadScene(nextSceneName);
    }
    /*
    private void lockingAllGoals() // invoke at the start to lock all goals
    {
        if (LockAllGoals)
        {
            foreach (scrGoal goal in goals)
            {
                goal.MovementLock = true;
            }

        }
    }
    */

    /*private bool checkIfAllGoalsReached()
    {
        //find all reached goals and check if all goals are unlocked
        List<scrGoal> reachedGoals = new List<scrGoal>();
        foreach (scrGoal goal in allGoals)
        {
            if (goal.checkIfReached() && !goal.Locked)
            {
                reachedGoals.Add(goal); 
            }
        }

        //check if all goals connects to the same scene.
        foreach(scrGoal goal in reachedGoals)
        {

        }


        return true;
    }
    */

    //call if all players are at the door and with no extra restriction 
    /*
    public void endScene(String nextScene)
    {
        if (nextScene == null)
            throw new NullReferenceException("Next Scene is not setted in inspector. Check player inspector");
        //go to next scene
        SceneManager.LoadScene(nextScene);
    }
    */
    private bool checkIfAllPlayerAtSameDoor()
    {
        print(0);
        //find all player and door object in scene
        GameObject[] arrDoor = GameObject.FindGameObjectsWithTag("goal");
        GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("player");

        List<scrGoal> reachedGoals = new List<scrGoal>();
        foreach (scrGoal goal in allGoals)
        {
            if (goal.checkIfReached() && !goal.Locked)
            {
                reachedGoals.Add(goal);
            }
        }

        //go through all doors and player, and see if each player has a unlocked + position matching door.
        foreach (var player in arrPlayer)
        {
            bool reached = false;
            foreach (var door in arrDoor)
            {
                print(1);
                //pass if player and door are not in the same panel
                if (player.transform.parent != door.transform.parent)
                    break;
                print(2);
                //pass (can't go to next level) if player is not at door
                if (player.GetComponent<GridObject>().gridPosition != door.GetComponent<GridObject>().gridPosition)
                    break;
                print(3);
                //pass if door is locked
                if (door.GetComponent<scrGoal>().Locked)
                    break;
                print(4);
                reached = true;
                reachedGoals.Add(door.GetComponent<scrGoal>());
            }
            print(5);
            if (!reached) return false; //if one player not reach unlocked door, then return false
        }
        print(6);
        //pass if not all reached doors connect to the same scene
        if (!reachedGoals.All(door => door.GetComponent<scrGoal>().nextSceneName == reachedGoals[0].GetComponent<scrGoal>().nextSceneName))
            return false;
        print(7);
        return true;//all challenge passed
    }
}
