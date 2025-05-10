using System.Collections;
using UnityEngine;

public class scrEnemyManager : MonoBehaviour
{
    [HideInInspector] public scrEnemy[] enemies;
    
    // keep in mind that enemies could be teleported to the scene EnemyManager's in!
    
    // 3. THIS IS THE FUNCTION called from somewhere else when WE, EnemyManager, IS ABOUT TO MOVE 
    public void MoveEnemies(Vector2 dir)
    {
        StartCoroutine(MoveEnemiesCoroutine(dir, FindAnyObjectByType<scrMoveInheritanceManager>().First_enemy_move_delay, FindAnyObjectByType<scrMoveInheritanceManager>().Enemies_move_delay));
    }

    private IEnumerator MoveEnemiesCoroutine(Vector2 dir, float initial_wait_time, float wait_time)
    {
        yield return new WaitForSeconds(initial_wait_time);
        
        // 1. Update enemies field
        enemies = RetrieveAllEnemies();

        // 2. Sort them then iterate over them
        enemies = sortEnemies(enemies);

        if (enemies.Length > 0)
        {
            // 3. foreach, call scrEnemy in a specific way (utilizing their respective scripts
            foreach (scrEnemy _enemy in enemies)
            {
                if (_enemy.GetComponent<scrDumbDumb>())
                {
                    _enemy.GetComponent<scrDumbDumb>().MoveDumbDumb(dir);
                }
                else if (_enemy.GetComponent<scrPatrol>())
                {
                    _enemy.GetComponent<scrPatrol>().MovePatrol();
                }
                else if (_enemy.GetComponent<scrStalkerFlashlight>())
                {
                    _enemy.GetComponent<scrStalkerFlashlight>().moveStalkerFlashlight();
                }
                else if (_enemy.GetComponent<scrStalker>())
                {
                    _enemy.GetComponent<scrStalker>().MoveStalker();
                }

                yield return new WaitForSeconds(wait_time);
            }
        }
    }
    
    
    // 1. set up a function for player to call that initiates a search for all enemies
    public scrEnemy[] RetrieveAllEnemies()
    {
        enemies = transform.parent.GetComponentsInChildren<scrEnemy>(false);
        return enemies;
    }
    
    // 2. a sort method that sorts enemies according to their priority int
    private scrEnemy[] sortEnemies(scrEnemy[] _enemies)
    {
        for (int i = 0; i < _enemies.Length - 1; i++)
        {
            for (int j = 0; j < _enemies.Length - i - 1; j++)
            {
                if (_enemies[j].priority > _enemies[j + 1].priority)
                {
                    scrEnemy temp = _enemies[j];
                    _enemies[j] = _enemies[j + 1];
                    _enemies[j + 1] = temp;
                }
            }
        }
        return _enemies;
    }
}
