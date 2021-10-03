using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] int _requiredHits = 3;
    int _currentHP;
    [SerializeField] TextMesh _textMesh;
    Rigidbody _rb;
    Transform _body;
    Vector3 _originalPos;
    bool _canTakeDamage;
    bool _isChopped = false;

    Transform _player;
   // [SerializeField] List <MeshFilter> _bodyMesh = new List<MeshFilter>();
   // [SerializeField] List<MeshFilter> _leavesMesh = new List<MeshFilter>();

    [Header("Body Parts")]
    [SerializeField] GameObject _logPrefab;
    [SerializeField] GameObject _logTop;
    [SerializeField] GameObject _logBase;
    List<GameObject> _logMiddleList = new List<GameObject>();
    int _logIndex = 0;

    [Header("FXs")]
    [SerializeField] GameObject _choppingFX;
    [SerializeField] List<ParticleSystem> _leavesFX = new List<ParticleSystem>();

    //[SerializeField] MeshFilter _treeBody;
    //[SerializeField] MeshFilter _treeLeaves;
    //Animator  _anim;

    [Header("Positions")]
    [SerializeField] Transform _middleLogPos;
    Vector3 _topLogPos = Vector3.zero;


    //Colors
    Color[] _colors = new Color[3];

    //Wood
    List<GameObject> _logList = new List<GameObject>();
    GameObject _woodPrefab;
    bool _startGathering = false;

    //Chainsaw
    bool _isUsingChainsaw = false;

    bool _isBonus = false;

    private void OnEnable()
    {
        //14, 8, 33
        _colors[0] = ColorList.colorList[14];
        _colors[1] = new Color(0,1,0,1);
        _colors[2] = ColorList.colorList[33];
        _isUsingChainsaw = false;

       // _treeLeaves.gameObject.GetComponent<MeshRenderer>().material.color = _colors[Random.Range(0, _colors.Length)];

        foreach (ParticleSystem particle in _leavesFX)
        {
           // particle.startColor = _treeLeaves.gameObject.GetComponent<MeshRenderer>().material.color;
        }

        _startGathering = false;
        _woodPrefab = Resources.Load<GameObject>("WoodPiece");
        
        //_rb = GetComponentInChildren<Rigidbody>();
        //_anim = GetComponent<Animator>();
        //_anim.enabled = true;

        BusSystem.OnNewLevelLoad += EnableTree;
        BusSystem.OnNewLevelStart += SetHP;
        BusSystem.OnTreeChopped += ChopSoundEffect;
        BusSystem.OnPhaseOneEnd += ChangeToBonusColor;

        _logList.Clear();
        HideText();

        _logIndex = 0;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelLoad -= EnableTree;
        BusSystem.OnNewLevelStart -= SetHP;
        BusSystem.OnTreeChopped -= ChopSoundEffect;
        BusSystem.OnPhaseOneEnd -= ChangeToBonusColor;
    }

    private void Awake()
    {
        //_body = transform.Find("Body");
        //_originalPos = _body.transform.localPosition;
        //_treeBody.sharedMesh = _bodyMesh[Random.Range(0, _bodyMesh.Count)].sharedMesh;
        //_treeLeaves.sharedMesh = _leavesMesh[Random.Range(0, _leavesMesh.Count)].sharedMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_CHAINSAW))
        {
            _isUsingChainsaw = true;

            for (int i = 0; i < _currentHP; i++)
                RemoveLog();

            ChopTree();
            CreateWood();

            _logTop.GetComponent<Rigidbody>().freezeRotation = false;
            _logTop.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void CreateLogs()
    {
        _logMiddleList.Clear();
        _currentHP = _requiredHits;
        _canTakeDamage = true;

        _logTop.GetComponent<Rigidbody>().isKinematic = true;
        _logTop.GetComponent<Rigidbody>().freezeRotation = true;
        _logBase.GetComponent<BoxCollider>().enabled = true;

        if (_requiredHits > 1)
        {
            _logTop.transform.localPosition += new Vector3(0, 2 *( _requiredHits - 1), 0);

            for (int i = 0; i < _requiredHits - 1; i++)
            {
                GameObject newLog = Instantiate(_logPrefab, _middleLogPos.position, _logPrefab.transform.localRotation);
                newLog.GetComponent<Rigidbody>().freezeRotation = true;
                newLog.GetComponent<Rigidbody>().isKinematic = true;
                newLog.transform.localPosition += new Vector3(0, 2 * i, 0);
                newLog.transform.parent = this.transform;
                newLog.tag = Const.TAG_TREEBODY;
                _logMiddleList.Add(newLog);
            }
        }

       // FunctionTimer.Create(() => _logTop.GetComponent<Rigidbody>().freezeRotation = false, 1f);
        //FunctionTimer.Create(() => _logTop.GetComponent<Rigidbody>().isKinematic = false, 1f);
    }

    void RemoveLog()
    {
        if (_currentHP >= 1)
        {
            if (_logIndex < _logMiddleList.Count && _logMiddleList.Count > 0)
            {
                _logMiddleList[_logIndex].GetComponent<Rigidbody>().freezeRotation = false;
                _logMiddleList[_logIndex].GetComponent<Rigidbody>().isKinematic = false;
                _logMiddleList[_logIndex].GetComponent<BoxCollider>().enabled = false;

                if (_isUsingChainsaw)
                     _logMiddleList[_logIndex].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-2,2), Random.Range(-2, 2), Random.Range(-2, 2)) * 400 * Time.deltaTime, ForceMode.Impulse);
                else
                    _logMiddleList[_logIndex].GetComponent<Rigidbody>().AddForce(new Vector3(-2, 0, 7) * 1600 * Time.deltaTime, ForceMode.Impulse);

                _logIndex++;

                if(!_isUsingChainsaw)
                {
                    if (_logIndex < _logMiddleList.Count)
                    {
                        _logMiddleList[_logIndex].GetComponent<Rigidbody>().isKinematic = false;
                        FunctionTimer.Create(() => _logMiddleList[_logIndex].GetComponent<Rigidbody>().AddForce(new Vector3(0, -4, 0) * 300 * Time.deltaTime, ForceMode.Impulse), .2f);
                    }
                    else
                    {
                        _logTop.GetComponent<Rigidbody>().isKinematic = false;
                        FunctionTimer.Create(() => _logTop.GetComponent<Rigidbody>().AddForce(new Vector3(0, -4, 0) * 300 * Time.deltaTime, ForceMode.Impulse), .2f);
                    }
                }   
            }
            else
            {
                _logTop.GetComponent<Rigidbody>().freezeRotation = false;
                _logTop.GetComponent<Rigidbody>().isKinematic = false;
                _logTop.GetComponent<BoxCollider>().enabled = false;
                _logTop.GetComponent<Rigidbody>().AddForce(new Vector3(-2, 0, 7) * 1000 * Time.deltaTime, ForceMode.Impulse);
            }
        } 
        else
            _logBase.GetComponent<BoxCollider>().enabled = false;
    }

    void SetHP()
    {
        CreateLogs();

        _isChopped = false;

        ShowText();
        SetText();

        //transform.localScale = new Vector3(1, Random.Range(.6f, 1.5f), 1);
       // transform.localScale += new Vector3(0, 1, 0)  * _requiredHits * .04f;
       // _textMesh.transform.localScale = Vector3.one * .6f;
    }

    public void TakeDamage()
    {
        RemoveLog();

        _currentHP--;
        _textMesh.text = _currentHP.ToString();
        //_anim.SetTrigger("isHit");
       
        BusSystem.CallSoundPlay((SoundEffects)Random.Range(3, 5));
        CreateWood();

       // _choppingFX.GetComponent<ParticleSystem>().Play();
       // foreach (Transform child in _choppingFX.transform)
          //  child.GetComponent<ParticleSystem>().Play();

        if (GameManager.Instance.canVibrate)
            Vibration.VibratePop();

        if (_currentHP <= 0)
            ChopTree();
    }

    void EnableTree()
    {

        //_treeBody.sharedMesh = _bodyMesh[Random.Range(0, _bodyMesh.Count)].sharedMesh;
        //_rb.isKinematic = true;
        //  _body.transform.localPosition = _originalPos;

        //foreach (Transform child in _logBodies)
        //  child.GetComponent<Rigidbody>().isKinematic = true;

    }

    public void ChopTree()
    {
        HideText();

        _isChopped = true;

        ApplyPhysics();
    }

    void ApplyPhysics()
    {
        //_anim.enabled = false;
        //_rb.isKinematic = false;
        //_rb.AddForce(Vector3.one * 70 * Time.deltaTime, ForceMode.Impulse);

        if (GameManager.Instance.canVibrate)  
            FunctionTimer.Create(() => Vibration.VibratePop(), 2f);

        BusSystem.CallTreeChopped();

        _logBase.GetComponent<BoxCollider>().enabled = false;
        _logBase.GetComponent<MeshRenderer>().enabled = false;

        if(_isBonus)
            BusSystem.CallAddCash(10 * _requiredHits);
    }

    public bool IsTreeChopped()
    {
        return _isChopped;
    }

    void ShowText()
    {
        _textMesh.GetComponent<MeshRenderer>().enabled = true;
        foreach (Transform child in _textMesh.transform)
            child.GetComponent<MeshRenderer>().enabled = true;
    }

    void HideText()
    {
        _textMesh.GetComponent<MeshRenderer>().enabled = false;
        foreach (Transform child in _textMesh.transform)
            child.GetComponent<MeshRenderer>().enabled = false;
    }

    void SetText()
    {
        // _textMesh = GetComponentInChildren<TextMesh>();
        _textMesh.text = _currentHP.ToString();
    }

    void ChopSoundEffect()
    {
        BusSystem.CallSoundPlay((SoundEffects)Random.Range(9, 11));
    }

    int HexToDec(string hex)
    {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    string DecToHex(int value)
    {
        return value.ToString("X2");
    }

    string FloatNormalizedToHex(float value)
    {
        return DecToHex(Mathf.RoundToInt(value * 255f));
    }

    float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex);
    }

    void CreateWood()
    {
        _player = GameObject.Find("Player").transform;
        BusSystem.CallTreeHit();

        for (int i = 0; i < 4; i++)
        {
            GameObject copyWood = Instantiate(_woodPrefab, transform.localPosition, Quaternion.identity);
            copyWood.transform.parent = this.transform;
            copyWood.transform.localPosition += new Vector3(-2.5f, 2.5f, 0);
            copyWood.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 500 * Time.deltaTime, ForceMode.Impulse);
            copyWood.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 500 * Time.deltaTime, ForceMode.Impulse);
            FunctionTimer.Create(() => Destroy(copyWood), 1f);
            // _logList.Add(copyWood);
        }

       // FunctionTimer.Create(() => GatherWood(), .6f);
    }

    void GatherWood()
    {
        for(int i = 0;  i < _logList.Count; i++)
        {
            _logList[i].GetComponent<Rigidbody>().isKinematic = true;
            _logList[i].transform.parent = _player.transform;
            //_logList[i].transform.localPosition = Vector3.zero;
            //_logList[i].transform.position += new Vector3(0.3f, 7f, -1.2f);
           /// FunctionTimer.Create(() => Destroy(_logList[i]), 1f);
        }
        FunctionTimer.Create(() => BusSystem.CallSoundPlay(SoundEffects.PickIngot), .6f);
        // FunctionTimer.Create(() => _startGathering = false, .6f);

        _startGathering = true;
    }

    void MoveLogs()
    {
        foreach(GameObject child in _logList)
        {
            child.transform.position = Vector3.MoveTowards(child.transform.position, _player.position + new Vector3(0, 5, 0), 3f * Time.deltaTime);
        }
    }

    void ChangeToBonusColor()
    {
        _isBonus = true;
        Color bonusColor = GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        foreach(Transform child in transform)
        {
            MeshRenderer childMesh = child.GetComponent<MeshRenderer>();
            if (childMesh != null)
                childMesh.material.color = bonusColor;
        }
    }
}
