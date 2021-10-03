using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YounGenTech.HealthScript;
using static Constants;

public class Player : Actor
{
    private Health healthScript;
    public Controls.BattleActions battleActions;
    [SerializeField] private GameObject playerActions;
    
    private Actor selectedEnemy;
    private int selectedEnemyNum = 0;
    private int attackCount = 0;

    public bool inAttack = false;
    /// <summary> The enemy that the player will attack </summary>
    public int SelectedEnemyNum {
        get
        {
            return selectedEnemyNum;
        }
        set
        {
            if (enemies.Length == 0)
            {
                return;
            }
            switch(value)
            {
                case 0:
                    selectedEnemy = enemies[0];
                    selectedEnemyNum = 0;
                    break;
                case 1:
                    selectedEnemy = enemies.Length < 2 ? enemies[0] : enemies[1];
                    selectedEnemyNum = enemies.Length < 2 ? 0 : 1;
                    break;
                case 2:
                    selectedEnemy = enemies.Length < 3 ? (enemies.Length < 2 ? enemies[0] : enemies[1]) : enemies[2];
                    selectedEnemyNum = enemies.Length < 3 ? (enemies.Length < 2 ? 0 : 1) : 2;
                    break;
                default:
                    Debug.LogError("SelectedEnemyNum is being set to in invalid number: " + value);
                    break;
            }
        }
    }

    /// <summary> Element meters of the player </summary>
    public int[] elementMeters = {0, 0, 0, 0, 0};

    private int[] elementExperience = {0, 0, 0, 0, 0};
    /// <summary> Array of experience for player's elements </summary>
    public int[] EXP {
        get
        {
            return elementExperience;
        }
        set
        {
            elementExperience = value;
        }
    }

    private int[] nextLevel = {100, 100, 100, 100, 100};
    /// <summary> Array that experience has to reach to level up </summary>
    public int[] MaxEXP {
        get
        {
            return nextLevel;
        }
        set
        {
            nextLevel = value;
        }
    }

    public override int HP {
        set
        {
            healthScript.Value = value;
            base.HP = value;
        }
    }

    public override int MaxHP {
        set
        {
            healthScript.MaxValue = value;
            base.MaxHP = value;
        }
    }

    private void Awake()
    {
        healthScript = GetComponent<Health>();
        
        battleActions = new Controls().Battle;

        battleActions.Physical.performed += ctx => Attack(Type.Physical);
        battleActions.Air.performed += ctx => Attack(Type.Air);
        battleActions.Water.performed += ctx => Attack(Type.Water);
        battleActions.Earth.performed += ctx => Attack(Type.Earth);
        battleActions.Fire.performed += ctx => Attack(Type.Fire);
        battleActions.Lightning.performed += ctx => Attack(Type.Electric);

        battleActions.DirectionalInput.performed += ctx => SelectedEnemyNum = ctx.ReadValue<float>() < 0 ? 2 : 0;
        battleActions.DirectionalInput.canceled += ctx => SelectedEnemyNum = 1;
    }

    private void Start()
    {
        GetEnemies();
    }

    public void AttackSelected()
    {
        Debug.Log("Player attacks!");
        battleActions.Enable();
        playerActions.SetActive(false);
    }

    public void Attack(Type type = Type.Physical)
    {
        if (inAttack)
            return;
        Debug.Log(type.ToString() + " attack performed against " + selectedEnemy.name + "!");
        this.type = type;
        StartCoroutine(AttackTimingCoroutine());
        selectedEnemy.TakeDamage(10, type);
        attackCount++;

        if (2 < attackCount)
        {
            battleManager.NextTurn();
            attackCount = 0;
        }
    }

    IEnumerator AttackTimingCoroutine()
    {
        inAttack = true;
        float totalAttackTime = 3.0f;
        float critWindowStart = 1f;
        float critWindowEnd = 2f;
        float currTime = 0f;
        while (currTime < totalAttackTime)
        {
            yield return 0;
            currTime += Time.deltaTime;
        }
        Debug.Log("finish");
        inAttack = false;
    }

    /// <summary> The player finds every active enemy on screen </summary>
    public override void GetEnemies()
    {
        GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            // Victory!
            battleManager.Victory();
        }
        if (enemies.Length != currentEnemies.Length)
        {
            enemies = new Actor[currentEnemies.Length];
        }

        for(int i = 0; i < currentEnemies.Length; i++)
        {
            enemies[i] = currentEnemies[i].GetComponent<Actor>();
        }

        // So that the currently selected enemy refreshes
        SelectedEnemyNum = SelectedEnemyNum;
    }
}
