//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;
using System.Collections;


public class CameraControl : MonoBehaviour
{
	public float smooth = 3f;		// カメラモーションのスムーズ化用変数
	public Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	public Transform frontPos;			// Front Camera locater

	// スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
	bool bQuickSwitch = false;  //Change Camera Position Quickly
    bool isfrontview = true;
	
	void Start()
	{
        transform.position = frontPos.position;
        transform.forward = frontPos.forward;
    }

	
	void FixedUpdate ()	// このカメラ切り替えはFixedUpdate()内でないと正常に動かない
	{

        if (Input.GetButtonDown("V"))	
		{
            if (!isfrontview)
            {
                setCameraPositionFrontView();
                setCameraPositionNormalView();
                setCameraPositionFrontView();
                isfrontview = true;
            }

            else
            {
                setCameraPositionNormalView();
                setCameraPositionFrontView();
                setCameraPositionNormalView();
                isfrontview = false;
            }
        }

        
	}

	void setCameraPositionNormalView()
	{
		if(bQuickSwitch == false){
		// the camera to standard position and direction
						transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
						transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
		}
		else{
			// the camera to standard position and direction / Quick Change
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
			bQuickSwitch = false;
		}
	}

	
	void setCameraPositionFrontView()
	{
		bQuickSwitch = true;
		transform.position = frontPos.position;	
	}
}
