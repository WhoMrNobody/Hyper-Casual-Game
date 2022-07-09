using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCyclinder : MonoBehaviour
{
    bool _filled;
    float _value;

    public void IncrementCyclinderVolume(float value){

        _value += value;
        if(_value > 1){

            float leftValue = _value - 1;
            int cylinderCount = PlayerController.Current.cyclinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.4f * (cylinderCount - 1) - 0.15f,transform.localPosition.z);
            transform.localScale= new Vector3( 0.4f , transform.localScale.y, 0.4f);
            PlayerController.Current.CreateCylinder(leftValue);

        }else if(_value < 0){

            PlayerController.Current.DestroyCylinder(this);
        }else{

            int cylinderCount = PlayerController.Current.cyclinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.4f * (cylinderCount - 1) - 0.15f * value,transform.localPosition.z);
            transform.localScale= new Vector3( 0.4f * value, transform.localScale.y, 0.4f * value);
        }
    }
}
