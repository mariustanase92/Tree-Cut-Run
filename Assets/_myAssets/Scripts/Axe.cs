using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{

    bool _canCut;
    bool _hitTree;

    [SerializeField] float _sizeChange = .7f;
    [SerializeField] float _bladeOffset = .5f;
    [SerializeField] float _maxCooldown = .6f;
    float _hitCooldown;

    [SerializeField] [Range(1, 50)] int _maxHP = 3;
    int _currHP = 3;

    TextMesh _textMesh;
    [SerializeField] Transform _hitBox;
    [SerializeField] Transform _bladePos;
    [SerializeField]Transform _blade;

    [SerializeField] GameObject _sizeFXs;

    private void OnEnable()
    {
        _canCut = false;
        _hitTree = false;
        _hitCooldown = _maxCooldown;

        BusSystem.OnNewLevelStart += Init;
        //BusSystem.OnLevelDone += SetCanCut;
        // BusSystem.OnCanCut += SetCanCut;
        // SetCanCut(false);
        ///SetCollider(false);
        
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= Init;
       // BusSystem.OnLevelDone -= SetCanCut;
       // BusSystem.OnCanCut -= SetCanCut;
    }

    private void Update()
    {
        if (_canCut)
            Attack();

        if(_hitTree)
        {
            _hitCooldown -= Time.deltaTime;

            if (_hitCooldown <= 0)
            {
                _hitTree = false;
                _hitCooldown = _maxCooldown;
            }        
        }
    }

    void Init()
    {
        _currHP = _maxHP;
        UpdateAxeText();
        
        //  SetCanCut(false);
    }

    public void UpdateAxeText()
    {
        if (_textMesh != null)
            _textMesh.text = _currHP.ToString();
    }

    public void DecreaseWeapon()
    {
        if (_currHP > 0)
        {
            _blade.transform.localScale -= new Vector3(1, 0, 1) * _sizeChange;

            if (_blade.transform.localScale.x < 1.3f)
                _blade.transform.localScale = new Vector3(1.3f, _blade.transform.localScale.y, _blade.transform.localScale.z);

            if (_blade.transform.localScale.x < 1.3f)
                _blade.transform.localScale = new Vector3(_blade.transform.localScale.x, _blade.transform.localScale.y, 1.3f);

            _blade.localPosition += new Vector3(_sizeChange * _bladeOffset, 0, 0);
            _currHP--;

            UpdateAxeText();

            if(_currHP == 0)
                transform.GetComponentInParent<CharacterMovement>().DisableAxeHP();
            else
                transform.GetComponentInParent<CharacterMovement>().EnableAxeHP();
        }
        else
            BusSystem.CallLevelDone(false);
    }

    public void IncreaseWeapon(int amount)
    {
        _blade.transform.localScale += new Vector3(1, 0, 1) * _sizeChange;

        Vector3 maxSize = Vector3.one * 3.5f;

        if (_blade.transform.localScale.x > maxSize.x)
            _blade.transform.localScale = new Vector3(maxSize.x, _blade.transform.localScale.y, _blade.transform.localScale.z);

        if (_blade.transform.localScale.x > maxSize.z)
            _blade.transform.localScale = new Vector3(_blade.transform.localScale.x, _blade.transform.localScale.y, maxSize.z);

        _blade.localPosition -= new Vector3(_sizeChange * _bladeOffset, 0, 0);

        _sizeFXs.GetComponent<ParticleSystem>().Play();
        foreach(Transform child in _sizeFXs.transform)
            child.GetComponent<ParticleSystem>().Play();

        _currHP += amount;
        _currHP = Mathf.Clamp(_currHP, 0, 50);
        UpdateAxeText();
    }

    public void SetCollider(bool enabled)
    {
       // _boxColl.enabled = enabled;  
    }

    public TextMesh SetTextMesh(TextMesh setText)
    {
        return _textMesh = setText;
    }

    public bool GetCanCut()
    {
        return _canCut;
    }

    public void SetCanCut(bool canCut)
    {
         _canCut = canCut;
       // FunctionTimer.Create(() => SetCollider(canCut), 1f);
    }

    public void Attack()
    {
        Collider[] hitSphere = Physics.OverlapSphere(_hitBox.position, 1.2f);
        for (int i = 0; i < hitSphere.Length; i++) 
        {
            Tree tree = hitSphere[i].GetComponent<Tree>();

            if (tree != null)
            {
                if (!_hitTree)
                {
                    _hitTree = true;
                    tree.TakeDamage();
                    DecreaseWeapon();
                }
            }
        } 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_hitBox.position, 1.2f);
    }

    public void SetHitBoxArea(Transform hitArea)
    {
        _hitBox = hitArea;
    }

    public int GetCurrentHP()
    {
        return _currHP;
    }
}
