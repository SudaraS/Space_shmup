using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eWeaponType{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
}

[System.Serializable]
public class WeaponDefinition{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the powerUp cube")]
    public string letter;
    [Tooltip("Color of powerUp cube")]
    public Color powerupColor = Color.white;
    [Tooltip("Prefab of weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab of the projectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the projectile that is fired")]
    public Color projectileColor = Color.white;
    [Tooltip("Damage caused when a single projectile hits an enemy")]
    public float damageonHit = 0;
    [Tooltip("Damage per second by the laser[Not Implemented]")]
    public float damagePerSec = 0;
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("Velocity of individual projectiles")]
    public float velocity = 50;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually while playing doesnt work properly")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime;

    private GameObject weaponModel;
    private Transform shotPointTrans;
    
    void Start()
    {
        if(PROJECTILE_ANCHOR == null){
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);

        SetType(_type);

        Hero hero = GetComponentInParent<Hero>();
        if(hero !=  null)hero.fireEvent += Fire;
    }

    public eWeaponType type{
        get{return(_type);}
        set{SetType(value);}
    }

    public void SetType(eWeaponType wt){
        _type = wt;
        if(type == eWeaponType.none){
            this.gameObject.SetActive(false);
            return;
        }
        else{
            this.gameObject.SetActive(true);
        }

        def = Main.GET_WEAPON_DEFINITION(_type);
        if(weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0;
    }

    private void Fire(){
        if(!gameObject.activeInHierarchy) return;
        if(Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch(type){
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
        }
    }

    private ProjectileHero MakeProjectile(){
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return(p);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
