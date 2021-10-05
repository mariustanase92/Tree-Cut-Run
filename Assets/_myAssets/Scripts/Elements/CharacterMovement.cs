using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //Scriptable Objects
    [SerializeField] CharacterDataSO dataSO;
    
    //Weapons
    [SerializeField] Transform _weaponMuzzle;
    [SerializeField] Transform _hitBoxArea;
    [SerializeField] GameObject _glowFXs;
    GameObject _axePrefab;
    GameObject _newAxe;
    GameObject _chainsawPrefab;
    GameObject _newChainsaw;
   
    //PlayMode ON/OFF
    bool _isPlaying = false;
    bool _canMove = true;

    //Transform
    Transform thisT;
    float currentSpeed;
    float startPos;
    float direction;

    //Camera
    [SerializeField] Camera dummmyCam;
    Vector3 camStartPos;

    //Animator
    Animator anim;
    
    //Chainsaw
    float _maxChainsawTime = 1f;
    float _chainsawTime = 3f;
    bool _isUsingChainsaw = false;
    bool _isInPhase2 = false;

    //Axe
    bool _isUsingAxe = false;
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
        
        DisableAxeHP();
   
    }

    private void OnEnable()
    {
        BusSystem.OnNewLevelStart += Init;
        BusSystem.OnNewLevelLoad += HandleNewLevelLoad;
        BusSystem.OnLevelDone += HandleLevelDone;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= Init;
        BusSystem.OnNewLevelLoad -= HandleNewLevelLoad;
        BusSystem.OnLevelDone -= HandleLevelDone;
    }

    void Update()
    {
        if (!_isPlaying) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            _canMove = !_canMove;
#endif
        if(_canMove)
        {
            thisT.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.World);
        }
            
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchBegan(dummmyCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                Input.mousePosition.y, dummmyCam.transform.position.z)) * -1);
        }
        if (Input.GetMouseButton(0))
        {
            HandleTouchMoved(dummmyCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                Input.mousePosition.y, dummmyCam.transform.position.z)) * -1);
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnded();
        }

        if(_isInPhase2)
        {

        }
        else if(_isUsingChainsaw)
        {
            _chainsawTime -= Time.deltaTime * .9f;
            
            UpdateHPText();

            if(_chainsawTime <= 0)
            {
                BusSystem.CallLevelDone(true);
            }        
        }

        if(_isUsingAxe)
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
            EnablePhase2();
            BusSystem.CallPhaseOneEnd();
            BusSystem.CallAddCash(GameManager.Instance.phase1Reward);
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
            if (!_isUsingChainsaw && !_isInPhase2)
            {
                if (other.GetComponent<Tree>().IsTreeChopped())
                {
                    _canMove = true;
                    _isUsingAxe = false;

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
                        _isUsingAxe = true;
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
            _isUsingAxe = false;
             
            if(_newAxe != null)
                _newAxe.GetComponent<Axe>().SetCanCut(false);

            if (!_isUsingChainsaw && !_isInPhase2 && _isPlaying)
                anim.Play(Const.WALK_ANIM);
        }
    }

    private void Init()
    {
        _isPlaying = true;
        _isUsingAxe = false;
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
        _isPlaying = false;

        transform.localEulerAngles = Vector3.zero;
        thisT.localPosition = new Vector3(0, 0, 0);
        thisT.GetChild(0).localEulerAngles = Vector3.zero;

        currentSpeed = dataSO.moveSpeed;

        anim.Play(Const.IDLE_ANIM);
        Camera.main.transform.position = camStartPos;

        DisableAxeHP();
    }

    private void HandleLevelDone(bool isWin)
    {
        _isPlaying = false;

        Camera.main.transform.parent = null;
        thisT.GetChild(0).localEulerAngles = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        
        DisablePhase2();
        DisableAxeHP();
        DestroyChainsaw();

        if (isWin)
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
        {
            Destroy(_newAxe);
        }        

        _axePrefab = Resources.Load<GameObject>("Axe");

        if(_axePrefab != null)
        {
            GameObject _copyAxe = Instantiate(_axePrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation);
            _newAxe = _copyAxe;
            _newAxe.transform.parent = _weaponMuzzle;
            _newAxe.transform.localPosition = new Vector3(-0.08f, 0, 0);
            _newAxe.transform.localEulerAngles = new Vector3(60, -180, 0);

            Axe _axeScript = _newAxe.GetComponent<Axe>();
            _axeScript.SetTextMesh(_axeHP.GetComponent<TextMesh>());
            _axeScript.SetHitBoxArea(_hitBoxArea);
            _axeScript.SetCurrentHP(dataSO.startHP);

            EnableAxeHP();
        }     
    }

    void CreateChainsaw()
    {
        if (_newChainsaw == null)
        {
            EnableAxeHP();

            _chainsawPrefab = Resources.Load<GameObject>("ChainsawWeapon");

            if(_chainsawPrefab != null)
            {
                GameObject _copyChainsaw = Instantiate(_chainsawPrefab, _weaponMuzzle.transform.position, _weaponMuzzle.transform.rotation);
                _newChainsaw = _copyChainsaw;
                _newChainsaw.transform.parent = _weaponMuzzle;
                _newChainsaw.transform.localPosition = new Vector3(-0.003f, 0.04f, -0.044f);
                _newChainsaw.transform.localEulerAngles = new Vector3(127.551f, 169.113f, -86.07999f);
                
                _glowFXs.SetActive(true);
            }
            

            if (_newAxe != null)
            {
                _chainsawTime = _newAxe.GetComponent<Axe>().GetCurrentHP();

                if (_chainsawTime <= 0)
                    _chainsawTime = 1;

                Destroy(_newAxe);
            }

            BusSystem.CallSoundPlay(SoundEffects.Chainsaw);
        }
    }

    void DestroyChainsaw()
    {
        _isUsingChainsaw = false;
        
        if(_newChainsaw != null)
        {
            Destroy(_newChainsaw);
        }
            
        if(_newAxe != null)
        {
            _newAxe.SetActive(true);
        }
    }

    void SetIsUsingChainsaw(bool useChainsaw)
    {
        _isUsingChainsaw = useChainsaw;

        anim.Play(Const.CHAINSAWWALK_ANIM);

        if (useChainsaw)
        {
            if(_newAxe != null)
                _newAxe.SetActive(false);
        }
    }

    void EnablePhase2()
    {
        SetIsUsingChainsaw(true);

        anim.Play(Const.CHAINSAWWALK_ANIM);
        CreateChainsaw();
    }

    void DisablePhase2(bool levelEnd = true)
    {
        _isInPhase2 = false;
    }

    public float GetCurrentHP()
    {
        return _chainsawTime;
    }

    public Transform GetAxeHP()
    {
        return _axeHP;
    }

    public void EnableAxeHP()
    {
        if(_axeHP.GetComponent<MeshRenderer>() != null)
        {
            _axeHP.GetComponent<MeshRenderer>().enabled = true;
        }
        
        if(_axeHP.GetChild(0) != null)
        {
            _axeHP.GetChild(0).gameObject.SetActive(true);
        }
       
        if (_newAxe != null)
        {
            _newAxe.GetComponent<Axe>().UpdateAxeText();
        }       
    }

    public void DisableAxeHP()
    {
        if (_axeHP.GetComponent<MeshRenderer>() != null)
        {
            _axeHP.GetComponent<MeshRenderer>().enabled = false;
        }

        if (_axeHP.GetChild(0) != null)
        {
            _axeHP.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void PlayHPFXs()
    {
        if (_axeHP.GetComponentInChildren<ParticleSystem>() != null)
        {
            _axeHP.GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    public void UpdateHPText()
    {
        if(_axeHP.GetComponent<TextMesh>() != null)
        {
            _axeHP.GetComponent<TextMesh>().text = Mathf.RoundToInt(_chainsawTime).ToString();
        } 
    }
}
