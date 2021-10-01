using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterDataSO dataSO;
    [SerializeField]
    private Camera dummmyCam;
    CapsuleCollider _capsule;

    //Weapons
    [SerializeField] Transform _weaponMuzzle;
    [SerializeField] Transform _hitBoxArea;
    [SerializeField] GameObject _glowFXs;
    GameObject _axePrefab;
    GameObject _newAxe;
    GameObject _chainsawPrefab;
    GameObject _newChainsaw;
   
    bool _isPlaying = false;
    bool _canMove = true;

    private Transform thisT;
    private float currentSpeed;
    private float startPos;
    private float direction;
    private Animator anim;
    private Vector3 camStartPos;

    [SerializeField] float _maxChainsawTime = 1f;
    float _chainsawTime = 3f;
    bool _isUsingChainsaw = false;
    bool _isInTurboMode = false;

    //Axe
    bool _isCutting = false;
    float _maxAxeTimer = .4f;
    float _axeTimer;

    //UI
    Transform _axeHP;
    

    private void Awake()
    {
        thisT = GetComponent<Transform>();
        anim = GetComponentInChildren<Animator>();
        currentSpeed = dataSO.moveSpeed;
        _axeHP = transform.Find("AxeHP");
        _capsule = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        BusSystem.OnNewLevelStart += Init;
        BusSystem.OnNewLevelLoad += HandleNewLevelLoad;
        BusSystem.OnNewLevelLoad += DisableAxeHP;
        BusSystem.OnLevelDone += HandleLevelDone;
        BusSystem.OnPhaseOneEnd += EnableTurboMode;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= Init;
        BusSystem.OnNewLevelLoad -= HandleNewLevelLoad;
        BusSystem.OnNewLevelLoad -= DisableAxeHP;
        BusSystem.OnLevelDone -= HandleLevelDone;
        BusSystem.OnPhaseOneEnd -= EnableTurboMode;
    }

    void Update()
    {
        if (!_isPlaying) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            _canMove = !_canMove;
#endif
        if(_canMove)
            thisT.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.World);

        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchBegan(dummmyCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dummmyCam.transform.position.z)) * -1);
        }
        if (Input.GetMouseButton(0))
        {
            HandleTouchMoved(dummmyCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dummmyCam.transform.position.z)) * -1);
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnded();
        }

        if(_isInTurboMode)
        {

        }
        else if(_isUsingChainsaw)
        {
            _chainsawTime -= Time.deltaTime;

            if(_chainsawTime <= 0)
                DestroyChainsaw();
        }

        Vector3 clampedPosition = thisT.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -dataSO.laneLimit, dataSO.laneLimit);
        thisT.localPosition = clampedPosition;

        if(_isCutting)
        {
            if(_newAxe != null)
            {
                _axeTimer -= Time.deltaTime;

                if(_axeTimer <= 0)
                {
                    _newAxe.GetComponent<Axe>().SetCanCut(true);
                    _axeTimer = _maxAxeTimer;
                }
            }            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.END_TAG))
        {
            if (!_isPlaying) return;

            bool isWin = true;
            BusSystem.CallLevelDone(isWin);

            
        }
        else if (other.CompareTag(Const.COLLECTIBLE_TAG))
        {
            Destroy(other.GetComponent<Collider>());
            other.GetComponent<Collectible>().Collect();
        }
        else if (other.CompareTag(Const.TAG_FUEL))
        {
            _chainsawTime = _maxChainsawTime;
            SetIsUsingChainsaw(true);
            CreateChainsaw();

            anim.Play(Const.CHAINSAWWALK_ANIM);

            if (other.GetComponent<Pickup>() != null)
                other.GetComponent<Pickup>().DisableMesh();

            BusSystem.CallSoundPlay(SoundEffects.PickChainsaw);
        }
        else if (other.CompareTag(Const.TAG_INGOT))
        {
            if (_newAxe != null)
                _newAxe.GetComponent<Axe>().IncreaseWeapon(other.GetComponent<Pickup>().GetRecoverHP());


            if (other.GetComponent<Pickup>() != null)
                other.GetComponent<Pickup>().DisableMesh();

            BusSystem.CallSoundPlay(SoundEffects.PickIngot);

            EnableAxeHP();
            PlayHPFXs();
        }
        else if (other.CompareTag(Const.TAG_CHECKPOINT))
        {
            FunctionTimer.Create(() => BusSystem.CallSoundPlay(SoundEffects.Checkpoint), .3f);
            EnableTurboMode();
            BusSystem.CallPhaseOneEnd();
        }
        else if (other.CompareTag(Const.TAG_OBSTACLE))
        {
            if(_isPlaying)
                BusSystem.CallLevelDone(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Const.TAG_TREEFRONT))
        {
            if (!_isUsingChainsaw && !_isInTurboMode)
            {
                if (other.GetComponent<Tree>().IsTreeChopped())
                {
                    _canMove = true;
                    _isCutting = false;

                    if(_isPlaying)
                        anim.Play(Const.WALK_ANIM);

                    if (_newAxe != null)
                        _newAxe.GetComponent<Axe>().SetCanCut(false);
                }
                else
                {
                    _canMove = false;

                    if (_isPlaying)
                    {
                        _isCutting = true;
                        anim.Play(Const.CHOP_ANIM);
                    }
                }
            }   
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Const.TAG_TREEFRONT))
        {
            _canMove = true;
            _isCutting = false;
             
            if(_newAxe != null)
                _newAxe.GetComponent<Axe>().SetCanCut(false);

            if (!_isUsingChainsaw && !_isInTurboMode && _isPlaying)
                anim.Play(Const.WALK_ANIM);
        }
    }

    private void Init()
    {
        _isPlaying = true;
        _isCutting = false;
         _canMove = true;

        GetComponent<Rigidbody>().isKinematic = false;
        anim.Play(Const.WALK_ANIM);
        camStartPos = Camera.main.transform.position;
        Camera.main.transform.SetParent(transform);
        _chainsawTime = _maxChainsawTime;
        _axeTimer = _maxAxeTimer;
        _glowFXs.SetActive(false);

        CreateAxe(); 
    }

    private void HandleNewLevelLoad()
    {
        transform.localEulerAngles = Vector3.zero;
        _isPlaying = false;

        thisT.localPosition = new Vector3(0, 0, 0);
        thisT.GetChild(0).localEulerAngles = Vector3.zero;

        currentSpeed = dataSO.moveSpeed;

        anim.Play(Const.IDLE_ANIM);
        Camera.main.transform.position = camStartPos;
    }

    private void HandleLevelDone(bool isWin)
    {
        Camera.main.transform.parent = null;
        thisT.GetChild(0).localEulerAngles = Vector3.zero;
        _isPlaying = false;

        GetComponent<Rigidbody>().isKinematic = true;
        DisableTurboMode();
       
        if(isWin)
        {
            Vibration.VibratePeek();
            BusSystem.CallSoundPlay(SoundEffects.Win);

            anim.Play(Const.WIN_ANIM);
            transform.localEulerAngles += new Vector3(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);
        }
            
        else
        {
            Vibration.VibrateNope();
            BusSystem.CallSoundPlay(SoundEffects.Lose);
            anim.Play(Const.LOSE_ANIM);
        }         
    }

    private void HandleTouchBegan(Vector3 position)
    {
        startPos = position.x;
        thisT.GetChild(0).localEulerAngles = Vector3.zero;
    }

    private void HandleTouchMoved(Vector3 position)
    {
        if (position.x == startPos) return;

        direction = position.x - startPos;
        thisT.localPosition += (Vector3.right * direction * dataSO.steerSpeed);
        startPos = position.x;

        float _angle = 30 * Mathf.Sign(direction);
        Vector3 _currentEuler = thisT.GetChild(0).localEulerAngles;
        float _targetEulerY = Mathf.LerpAngle(_currentEuler.y, _angle, Time.deltaTime * 3f);
        thisT.GetChild(0).localEulerAngles = Vector3.up * _targetEulerY;
    }

    private void HandleTouchEnded()
    {
        thisT.GetChild(0).localEulerAngles = Vector3.zero;
    }


    void CreateAxe()
    {
        if (_newAxe != null)
            Destroy(_newAxe);

        EnableAxeHP();
        _axePrefab = Resources.Load<GameObject>("Axe");
        GameObject _copyAxe = Instantiate(_axePrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation);
        _newAxe = _copyAxe;
        _newAxe.transform.parent = _weaponMuzzle;
        _newAxe.transform.localPosition = new Vector3(-0.08f, 0, 0);
        _newAxe.transform.localEulerAngles = new Vector3(60, -180, 0);
        _newAxe.GetComponent<Axe>().SetTextMesh(_axeHP.GetComponent<TextMesh>());
        _newAxe.GetComponent<Axe>().SetHitBoxArea(_hitBoxArea);
    }

    void CreateChainsaw()
    {
        if (_newChainsaw == null)
        {
            _chainsawPrefab = Resources.Load<GameObject>("ChainsawWeapon");
            GameObject _copyChainsaw = Instantiate(_chainsawPrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation);
            _newChainsaw = _copyChainsaw;
            _newChainsaw.transform.parent = _weaponMuzzle;
            _newChainsaw.transform.localPosition = new Vector3(-0.003f, 0.04f, -0.044f);
            _newChainsaw.transform.localEulerAngles = new Vector3(127.551f, 169.113f, -86.07999f);
            _glowFXs.SetActive(true);
        }

        BusSystem.CallSoundPlay(SoundEffects.Chainsaw);
    }

    void DestroyChainsaw()
    {
        _isUsingChainsaw = false;
        Destroy(_newChainsaw);

        if(_newAxe != null)
        {
            _newAxe.SetActive(true);
            EnableAxeHP();
        }
        
        if(_isPlaying)
            anim.Play(Const.WALK_ANIM);

        BusSystem.CallSoundPlay(SoundEffects.Hit1);
    }

    void SetIsUsingChainsaw(bool useChainsaw)
    {
        _isUsingChainsaw = useChainsaw;
        anim.Play(Const.CHAINSAWWALK_ANIM);

        if (useChainsaw)
        {
            DisableAxeHP();
            _newAxe.SetActive(false);
        }
    }

    void EnableTurboMode()
    {
        _isInTurboMode = true;
        DisableAxeHP();
        Destroy(_newAxe);
        anim.Play(Const.CHAINSAWWALK_ANIM);
        CreateChainsaw();
    }

    void DisableTurboMode(bool levelEnd = true)
    {
        _isInTurboMode = false;
        DestroyChainsaw();
    }

    public Transform GetAxeHP()
    {
        return _axeHP;
    }

    public void EnableAxeHP()
    {
        _axeHP.GetComponent<MeshRenderer>().enabled = true;
        _axeHP.GetChild(0).gameObject.SetActive(true);
        FunctionTimer.Create(() => _newAxe.GetComponent<Axe>().UpdateAxeText(), .1f);
    }

    public void DisableAxeHP()
    {
        _axeHP.GetComponent<MeshRenderer>().enabled = false;
        _axeHP.GetChild(0).gameObject.SetActive(false);
    }

    public void PlayHPFXs()
    {
        _axeHP.GetComponentInChildren<ParticleSystem>().Play();
    }

}
