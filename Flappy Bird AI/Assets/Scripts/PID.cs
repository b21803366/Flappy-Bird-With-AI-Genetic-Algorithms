using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PID 
{ 
    public float _Kp;
    private float _Ki;
    private float _Kd;
 
    private float _dt;
    public float _setPoint;
 
    private float _pre_error;
    private float _integral;
 
    private float _max;
    private float _min;
    private float _err;
    private float _p_contrl;
    private float _i_contrl;
    private  float _i_err_max_limit=20;
    public float totalErrSquereSum=0;
    private float _err_sum;
     private  float _d_contrl;
     private float _err_diff;
     private float _err_last;
     private float Time_Step=0;
     private float _Output;
    public PID(float Kp, float Ki, float Kd, float Time_Steps)
{
         _Kp = Kp;
        _Ki = Ki;
         _Kd = Kd;
        Time_Step=Time_Steps;
        //_dt = dt;
       // _setPoint = setValue;
        
        _pre_error = 0;
         _integral = 0;
        _err_sum=0;
        _err_diff=0;
        _err_last=0;
        //_max = max;
        //_min = min;
        }
    public void Set(float lineCoords,float birdCoords){
        _setPoint=lineCoords;
        _err=_setPoint-birdCoords;

    }
    public void Calculate(){
        //proportional
        totalErrSquereSum+=_err*_err;
        _p_contrl= _Kp* _err;
        //integral 
        if(_err<_i_err_max_limit){
        _err_sum+=_err *Time_Step;}
        
        _i_contrl= _Ki* _err_sum;
        //deviration
        _err_diff=(_err-_err_last)/Time_Step;
        _d_contrl=_Kd* _err_diff;
        _err_last=_err;
    }
    public float sendOut(){
        _Output=_p_contrl+ _i_contrl + _d_contrl;
        return _Output;
    }
    public float normalize(float x){
        _max=200;
        _min=0;
        if (x>_max){
            x=_max;
        }else if(x<_min){
            x=_min;
        }
        float normalize=( x -_min)/(_max- _min);
        return normalize;
    }
    public float getErr(){
        return _err;
    }
    }
