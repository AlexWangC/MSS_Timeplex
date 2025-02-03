using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrGoalManager : MonoBehaviour
{
    //public bool LockAllGoals; // whether you want to have all goals be locked in the scene. NOT YET IMPLEMENTED
    private scrGoal[] goals;

    private void Start()
    {
        initializeGoals();
        Debug.Log("Next Scene Index = " + (SceneManager.GetActiveScene().buildIndex + 1));
        //lockingAllGoals();
    }

    public void GoalsReached() //subscribe this to an event maybe, or call it from somewhere else
    {
        Debug.Log("Goals Reached check invoked...");
        if (checkIfAllGoalsReached())
        {
            //play sound
            scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.level_clear, this.transform, 1);
            
            Debug.Log("Hey all goals have been reached");
            // content here
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene
        }
    }

    private void initializeGoals()
    {
        goals = FindObjectsByType<scrGoal>(FindObjectsSortMode.None);
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
    
    private bool checkIfAllGoalsReached()
    {
        goals = FindObjectsByType<scrGoal>(FindObjectsSortMode.None);
        foreach (scrGoal goal in goals)
        {
            if (goal.Reached == false)
            {
                return false;
            }
        }

        return true;
    }
}
