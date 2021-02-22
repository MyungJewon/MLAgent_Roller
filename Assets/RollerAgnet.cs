using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
//������Ʈ�� ���� ��ũ��Ʈ

//����ϱ����� MonoBehaviour ���� Agent �κ���
public class RollerAgnet : Agent
{
    Rigidbody rBody;

    //Ÿ�ٿ�����Ʈ�� �ޱ����� �ʵ� ����
    public Transform Target;

    //Rigidbody Component �߰�
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }



    /*OnEpisodeBegin �н��� �����Ҷ� ���� �ѹ��� ȣ��Ǵ� �޼ҵ�
      ������Ʈ�� �ʱ�ȭ �� ȯ���� �����Ѵ�.*/
    public override void OnEpisodeBegin()
    {

        //������Ʈ�� �������� ������� 0���� ����
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        //Ÿ���� ���ο� �������� �̵�
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }



    /*CollectObservations ������Ʈ�� ���������� �����ϰ� 
      AddObservation �޼ҵ带 ����� �����Ѵ�*/
    public override void CollectObservations(VectorSensor sensor)
    {
        //Ÿ�ٰ� ������Ʈ�� ��ġ ���� 
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // ������Ʈ �ӵ� ����
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }


    /*OnActionReceived �۾��� �ϰ� ������ �Ҵ��ϴ� �޼ҵ�
      ��ȭ�н����� �켱������ �����Ҽ��ִ� �߿��� �޼ҵ�*/
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //������Ʈ�� �̵� ����
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Ÿ���� ��ġ�� �̵���������� ��ġ�� �ޱ�
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // ��ǥ���޽� ���� ���� �� ���Ǽҵ� ����
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // ������Ʈ�� �������� ��ü������ �缳���Ҽ��ְ� ����
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
