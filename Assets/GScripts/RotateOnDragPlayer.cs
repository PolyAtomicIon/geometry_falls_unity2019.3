using UnityEngine;
 using System.Collections;
 
 public class RotateOnDragPlayer : MonoBehaviour 
 {
     
    public float strength = 5;
    
    public float Speed = 2;
    float AngleY;

    public bool dragging = false;

    void Update(){

        // if( dragging ){

        //     float zCam = -Camera.main.transform.position.z;
        //     Vector3 mouseVector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zCam);
        //     Vector3 mPos = Camera.main.ScreenToWorldPoint(mouseVector);
            
        //     Quaternion targetRotation = Quaternion.LookRotation (mPos - transform.position);
        //     targetRotation.x = 0.0f;
        //     targetRotation.z = 0.0f;
            
        //     Debug.Log(mPos.y - transform.position.y);

        //     float str = Mathf.Min (strength * Time.deltaTime, 1);
        //     transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);                

        // }

        if ( dragging ){
            
            Vector3 mouseScreenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z );
            
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint( mouseScreenPosition );

            mouseWorldPosition.y = Mathf.Min(8.5f, mouseWorldPosition.y);

            Debug.Log(mouseWorldPosition);

            if( Mathf.Abs(0f - Mathf.Abs(mouseWorldPosition.x)) <= 0.5f && Mathf.Abs(6.2f + mouseWorldPosition.z) <= 0.5f){
                mouseWorldPosition = new Vector3(0.0f, 9.9f, -6.2f);
            }

            Debug.Log(mouseWorldPosition);
            
            // transform.LookAt( mouseWorldPosition,  Vector3.up);

            // Vector3 direction = mouseWorldPosition - transform.position;
            // Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            Vector3 relativePos = mouseWorldPosition - transform.position;
            if( relativePos.magnitude > 0f ){
                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                // StartCoroutine( Rotate(rotation) );
                transform.rotation = rotation;
            }

        }

    }

    public void RotateY(){
        AngleY += Input.GetAxis("Mouse Y") * Speed * Time.deltaTime;
        
        // Upward Sections
        AngleY = Mathf.Clamp(AngleY, -180, 0);
        
        transform.localRotation = Quaternion.AngleAxis(AngleY, Vector3.right);
    }

    //  private float _sensitivity;
    //  private Vector3 _mouseReference;
    //  private Vector3 _mouseOffset;
    //  private Vector3 _rotation;
    //  private bool _isRotating;
     
    //  void Start ()
    //  {
    //      _sensitivity = 0.25f;
    //      _rotation = Vector3.zero;
    //  }
     
    //  void Update()
    //  {
    //      if(_isRotating)
    //      {
    //          // offset
    //          _mouseOffset = (Input.mousePosition - _mouseReference);
    //          // apply rotation
    //          //_rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;
    //          _rotation.y = (_mouseOffset.x) * _sensitivity;
    //         //  _rotation.x = (_mouseOffset.y) * _sensitivity;
    //          // rotate
    //          //transform.Rotate(_rotation);
    //          transform.eulerAngles += _rotation;
    //          // store mouse
    //          _mouseReference = Input.mousePosition;
    //      }
    //  }
     
    //  void OnMouseDown()
    //  {
    //      // rotating flag
    //      _isRotating = true;
         
    //      // store mouse
    //      _mouseReference = Input.mousePosition;
    //  }
     
    //  void OnMouseUp()
    //  {
    //      // rotating flag
    //      _isRotating = false;
    //  }
     
 }