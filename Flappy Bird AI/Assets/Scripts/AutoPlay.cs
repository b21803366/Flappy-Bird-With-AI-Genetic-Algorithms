using UnityEngine;
public class AutoPlay 
{

public float[] randomKs=new float[3];
public PID pid;
public float normal_result;
public float output_pid;
public float jp;
public float red;
    public AutoPlay(float p,float i,float d){
        randomKs[0]=p;
        randomKs[1]=i;
        randomKs[2]=d;
    }
    
  public void auto(){
       
        
        pid=new PID(randomKs[0],randomKs[1],randomKs[2],Time.deltaTime);


  }
  public void set(float x,float y){
      pid.Set(x,y);
  }
  public void Calculate(){
      pid.Calculate();
  }
  public void output(){
      output_pid=pid.sendOut();
  }
  public void normal(){
       normal_result=pid.normalize(output_pid);
  }
  public void getJP(){
      jp=pid._setPoint-output_pid;
  }
  public void getRed(int targetF,int cps){
      red=normal_result*cps;//*60;
    red=red/ targetF;
    red=Mathf.Ceil(red);
  }
  public double fitnes(PID pid,double time){
      double sumSq=(double)pid.totalErrSquereSum;
      double result=sumSq/time;
      return result;
  }
  public void Click(int i){
      if(normal_result>0){
          if(red!=0){
                if (i == 0)
                    for (int t = 0; t < red; t++)
                        Bird_AI.GetInstance().touchCount++;
          }
      }
  }

  public void work(float x,float y,int targetF,int cps,int i){
      auto();
      set(x,y);
      Calculate();
      output();
      normal();
      getJP();
      getRed(targetF,cps);
      Click(i);

  }
}
