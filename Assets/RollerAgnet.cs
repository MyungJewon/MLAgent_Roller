using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
//에이전트에 들어가는 스크립트

//상속하기위해 MonoBehaviour 에서 Agent 로변경
public class RollerAgnet : Agent
{
    Rigidbody rBody;

    //타겟오브젝트를 받기위한 필드 선언
    public Transform Target;

    //Rigidbody Component 추가
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }



    /*OnEpisodeBegin 학습이 시작할때 마다 한번식 호출되는 메소드
      에이전트의 초기화 및 환경을 제어한다.*/
    public override void OnEpisodeBegin()
    {

        //에이전트가 떨어지면 모멘텀을 0으로 절정
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        //타겟을 새로운 지점으로 이동
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }



    /*CollectObservations 에이전트의 관측정보를 수집하고 
      AddObservation 메소드를 사용해 전송한다*/
    public override void CollectObservations(VectorSensor sensor)
    {
        //타겟과 에이전트의 위치 전송 
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // 에이전트 속도 전송
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }


    /*OnActionReceived 작업을 하고 보상을 할당하는 메소드
      강화학습에서 우선순위를 설정할수있는 중요한 메소드*/
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //오브젝트의 이동 설정
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // 타겟의 위치로 이동햇을경우의 위치값 받기
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // 목표도달시 보상 증정 및 에피소드 종료
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // 에이전트가 떨어졋을경우 자체적으로 재설정할수있게 조정
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
