using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public List<Transform> targets = new();

    private readonly bool canGrow = true;
    private readonly List<GameObject> hotKeys = new();
    private int amountOfAttacks = 4;

    private float blackholeTimer;


    private bool canCreateHotKey = true;
    private bool canShrink;
    private float cloneAttackCooldown = .3f;

    private bool cloneAttackReleased;
    private float cloneAttackTimer;
    private float growSpeed;

    private float maxSize;
    private bool playerCanDissapear = true;
    private float shrinkSpeed;

    public bool playerCanExitState { get; private set; }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer <= 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHole();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize),
                Time.deltaTime * growSpeed);
        }

        if (canShrink)
        {
            transform.localScale =
                Vector2.Lerp(transform.localScale, new Vector2(-1, -1), Time.deltaTime * shrinkSpeed);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<Enemy>()?.FreezeTime(false);
    }


    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks,
        float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (SkillManager.instance.clone.crystalInstead)
        {
            playerCanDissapear = false;
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
        {
            return;
        }

        DestroyAllHotKeys();
        cloneAttackReleased = true;
        canCreateHotKey = false;

        if (playerCanDissapear)
        {
            playerCanExitState = false;
            PlayerManager.instance.player.fx.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            var randomIndex = Random.Range(0, targets.Count);
            float xOffset;

            if (Random.Range(0f, 1f) > .5f)
            {
                xOffset = 2;
            }
            else
            {
                xOffset = -2;
            }

            if (SkillManager.instance.clone.crystalInstead)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHole", 1f);
            }
        }
    }

    private void FinishBlackHole()
    {
        playerCanExitState = true;
        // PlayerManager.instance.player.ExitBlackhole();
        cloneAttackReleased = false;
        canShrink = true;
        DestroyAllHotKeys();
        PlayerManager.instance.player.fx.MakeTransparent(false);
    }

    private void DestroyAllHotKeys()
    {
        if (hotKeys.Count <= 0)
        {
            return;
        }

        for (var i = 0; i < hotKeys.Count; i++)
        {
            Destroy(hotKeys[i]);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0 || !canCreateHotKey)
        {
            return;
        }

        var newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        hotKeys.Add(newHotKey);

        var choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        var newHoleHotKeyScript = newHotKey.GetComponent<BlackHoleHotKeyController>();

        newHoleHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform)
    {
        targets.Add(_enemyTransform);
    }
}