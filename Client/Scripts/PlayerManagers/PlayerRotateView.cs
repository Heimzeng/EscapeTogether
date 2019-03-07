using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class PlayerRotateView : MonoBehaviour {

    private bool istouchingroate = false;
    private bool isMovedtoRoate = false;
    private int roatetouchingindex = 0;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
    public string m_PlayerInput;
    public float sensitivityX = 10F;
	public float sensitivityY = 10F;
    public Vector3 roate;

    public float minimumX = -100F;
	public float maximumX = 100F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public float rotationY = 0F;
    public float rotationX = 0F;

	void Update ()
	{
        roate = transform.localEulerAngles;

        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.touches[i].phase == TouchPhase.Began)
            {
                Vector2 touchpos = Input.touches[i].position;
                if (isTouchInRoateare(touchpos))
                {
                    roatetouchingindex = i;
                    istouchingroate = true;
                }
            }
            if (Input.GetTouch(roatetouchingindex).phase == TouchPhase.Moved)
            {
                isMovedtoRoate = true;
                rotationX = Input.GetTouch(roatetouchingindex).deltaPosition.x * Time.deltaTime * sensitivityX;
                rotationY += Input.GetTouch(roatetouchingindex).deltaPosition.y * Time.deltaTime * sensitivityY;
             
            }
            else
            {
                rotationX = 0f;
                isMovedtoRoate = false;
            }
            if (Input.GetTouch(roatetouchingindex).phase == TouchPhase.Ended)
            {
                istouchingroate = false;
               
            }

            
        }
       // rotationX = Input.GetAxis("Mouse X " + m_PlayerInput) * sensitivityX;
      //  rotationY += Input.GetAxis("Mouse Y " + m_PlayerInput) * sensitivityY;
        if (istouchingroate && isMovedtoRoate)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                rotationX += transform.localEulerAngles.y;

                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                transform.Rotate(0, rotationX, 0);
            }
            else
            {
               rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }
		
	}
    private bool isTouchInRoateare(Vector2 pos)
    {
        if (pos.x < Screen.width / 2) return false;
        return true;
    }
    void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

    public void RoateByX(float IrotationX)
    {
        transform.localEulerAngles = new Vector3(0, IrotationX, 0);
    }
}