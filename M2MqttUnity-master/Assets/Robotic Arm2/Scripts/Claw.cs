using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Claw : MonoBehaviour
{
    [SerializeField] Animator ClawAnimator;
    [SerializeField] Slider[] Robotic_slider;
    [SerializeField] GameObject[] Robotic_obj;
    [SerializeField] Text[] RotationText;

    [Header("Auto Control")]
    public Transform pickPoint;
    public Transform placePoint;
    public Transform homePoint;
    public Transform grabbedObject;

    public float moveSpeed = 2f;

    bool IsPressed = false;
    bool isMoving = false;

    Button ClawButton;

    void Start()
    {
        ClawAnimator.enabled = false;

        for (int i = 0; i < Robotic_slider.Length; i++)
        {
            int index = i;
            Robotic_slider[i].onValueChanged.AddListener(delegate {
                OnSliderChanged(Robotic_slider[index].value, index, 180);
            });
        }

        PickObject();
    }

    void Update()
    {
        for (int i = 0; i < RotationText.Length; i++)
        {
            RotationText[i].text = Robotic_slider[i].value.ToString("F1");
        }
    }

    // 🔹 수동 Open/Close
    public void OpenClose()
    {
        ClawAnimator.enabled = true;

        if (!IsPressed)
        {
            ClawAnimator.SetBool("IsOpen", true);
            IsPressed = true;
        }
        else
        {
            ClawAnimator.SetBool("IsOpen", false);
            IsPressed = false;
        }
    }
    
    // 🔥 자동 공정 시작
    public void PickObject()
    {
        if (!isMoving)
        {
            StartCoroutine(PickSequence());
        }
    }

    void OnSliderChanged(float value, int i, float rotationvalue)
    {
        if (i == 4 || i == 0)
        {
            Robotic_obj[i].transform.localRotation =
                Quaternion.Euler(0, Robotic_slider[i].value, 0);
        }
        else
        {
            Robotic_obj[i].transform.localRotation =
                Quaternion.Euler(0, 0, Robotic_slider[i].value);
        }
    }

    public void SetSliderValue(int index, float value)
    {
        Robotic_slider[index].value = value;
    }
    
    float[] CalculateIK(Vector3 target)
    {
        float Pi = Mathf.PI;

        float BASE_HEIGHT = 100f;
        float L1 = 120f; // shoulder
        float L2 = 120f; // arm
        float L3 = 140f; // grip

        float X = target.x;
        float Y = target.z;
        float Z = target.y;
        
        // 🔹 평면 거리
        float R = Mathf.Sqrt(X * X + Y * Y);

        // 🔹 손목 목표 위치 보정
        float GRIP_ANGLE = 90 * Mathf.Deg2Rad;

        float wristR = R - Mathf.Sin(GRIP_ANGLE) * L3;
        float wristZ = Z - BASE_HEIGHT + Mathf.Cos(GRIP_ANGLE) * L3;

        float dist = Mathf.Sqrt(wristR * wristR + wristZ * wristZ);

        // ❗ 도달 불가 체크
        if (dist > (L1 + L2))
        {
            Debug.LogError("❌ IK 실패: 도달 불가능한 위치");
            return null;
        }

        // 🔹 코사인 법칙 (더 안정적)
        float cosElbow = (L1 * L1 + L2 * L2 - dist * dist) / (2 * L1 * L2);
        cosElbow = Mathf.Clamp(cosElbow, -1f, 1f);

        float ELBOW_ANGLE = Mathf.Acos(cosElbow);

        float k1 = L1 + L2 * Mathf.Cos(ELBOW_ANGLE);
        float k2 = L2 * Mathf.Sin(ELBOW_ANGLE);

        float SHOULDER_ANGLE = Mathf.Atan2(wristZ, wristR) - Mathf.Atan2(k2, k1);

        float WRIST_ANGLE = GRIP_ANGLE - SHOULDER_ANGLE - ELBOW_ANGLE;
        
        float shoulderDeg = SHOULDER_ANGLE * Mathf.Rad2Deg;
        float elbowDeg = ELBOW_ANGLE * Mathf.Rad2Deg;
        float wristDeg = WRIST_ANGLE * Mathf.Rad2Deg;

        // 👉 분산 (너가 원한 방식)
        shoulderDeg *= 0.7f;
        elbowDeg *= 0.7f;
        wristDeg *= 0.6f;

        // 👉 제한 (-50 ~ 50)
        shoulderDeg = Mathf.Clamp(shoulderDeg, -50f, 50f);
        elbowDeg = Mathf.Clamp(elbowDeg, -50f, 50f);
        wristDeg = Mathf.Clamp(wristDeg, -50f, 50f);

        return new float[]
        {
            0f,
            shoulderDeg,
            elbowDeg,
            wristDeg,
            90f
        };
    }
    
    void ApplyIK(float[] angles)
    {
        // 1️⃣ 어깨
        Robotic_obj[1].transform.localRotation =
            Quaternion.Euler(0, 0, angles[1]);

        // 2️⃣ 팔
        Robotic_obj[2].transform.localRotation =
            Quaternion.Euler(0, 0, -angles[2]);

        // 3️⃣ 손목
        Robotic_obj[3].transform.localRotation =
            Quaternion.Euler(0, 0, -angles[3]);
        
        // 4️⃣ 집게 방향
        Robotic_obj[4].transform.localRotation =
            Quaternion.Euler(0, angles[4], 0);
    }
    
    IEnumerator MoveToIK(Vector3 target)
    {
        float[] angles = CalculateIK(target);

        float time = 0f;
        float duration = 1f;

        float[] startAngles = new float[5];

        // 현재 각도 직접 가져오기
        startAngles[0] = Robotic_obj[0].transform.localEulerAngles.y;
        startAngles[1] = Robotic_obj[1].transform.localEulerAngles.z;
        startAngles[2] = Robotic_obj[2].transform.localEulerAngles.z;
        startAngles[3] = Robotic_obj[3].transform.localEulerAngles.z;
        startAngles[4] = Robotic_obj[4].transform.localEulerAngles.y;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float[] current = new float[5];

            for (int i = 0; i < 5; i++)
            {
                current[i] = Mathf.LerpAngle(startAngles[i], angles[i], t);
            }

            ApplyIK(current);

            yield return null;
        }
    }
    
    IEnumerator PickSequence()
    {
        isMoving = true;

        Vector3 pick = pickPoint.position;
        Vector3 place = placePoint.position;
        Vector3 home = homePoint.position;

        Vector3 pickUp = pick + Vector3.up * 30f;
        Vector3 placeUp = place + Vector3.up * 30f;
        
        // 0️⃣ 몸통 (여기는 상태따라 집을때 0, 우측으로 옮길때 -180, 되돌아올때 0)
        Robotic_obj[0].transform.localRotation =
            Quaternion.Euler(0, 0, 0);
        
        yield return MoveToIK(pickUp);

        isMoving = false;
    }
    
    void RotateBaseTowards(Vector3 target)
    {
        // 기준점 (몸통 위치)
        Vector3 basePos = Robotic_obj[5].transform.position;

        // 방향 벡터
        Vector3 dir = target - basePos;

        // Y축 회전만 사용 (수평 회전)
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        // 적용
        Robotic_obj[5].transform.localRotation =
            Quaternion.Euler(0, angle, 0);
    }
}