using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimMath
{

    public static float Map(float v, float mina, float maxa, float minb, float maxb) {
        float p = (v - mina) / (maxa - mina);
        return Lerp(minb, maxb, p);
    }

    public static float Lerp(float a, float b, float p, bool allowExtrapolation = true) {
        
        if(!allowExtrapolation) {
            if (p > 1) p = 1;
            if (p < 0) p = 0;
        }

        return (b - a) * p + a;
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float p, bool allowExtrapolation = true) {

        if (!allowExtrapolation)
        {
            if (p > 1) p = 1;
            if (p < 0) p = 0;
        }

        return (b - a) * p + a;
    }

    public static Quaternion Lerp(Quaternion a, Quaternion b, float p, bool allowExtrapolation = false) {


        b = WrapQuaternion(a, b); // Wraps b to be less tha 180 degrees. Fixes glitch where rotations spin the wrong way

        Quaternion rot = Quaternion.identity;

        if (!allowExtrapolation)
        {
            if (p > 1) p = 1;
            if (p < 0) p = 0;
        }

        rot.x = Lerp(a.x, b.x, p, allowExtrapolation);
        rot.y = Lerp(a.y, b.y, p, allowExtrapolation);
        rot.z = Lerp(a.z, b.z, p, allowExtrapolation);
        rot.w = Lerp(a.w, b.w, p, allowExtrapolation);

        return rot;
    }

    // Float Easing
    public static float Ease(float current, float target, float percentLeftAfter1Second, float dt = -1) {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, dt);
        return Lerp(current, target, p);

    }

    // Vector3 Easing
    public static Vector3 Ease(Vector3 current, Vector3 target, float percentLeftAfter1Second, float dt = -1) {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return Lerp(current, target, p);

    }

    // Quaternion Easing
    public static Quaternion Ease(Quaternion current, Quaternion target, float percentLeftAfter1Second, float dt = -1)
    {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return Lerp(current, target, p);

    }

    /// <summary>
    /// Trying to ease between angles > 180 degrees? you need to wrap your angles!
    /// </summary>
    /// <param name="baseAngle">This angle won't change</param>
    /// <param name="angleToBeWrapped">This angle will change so that is is relative to baseAngle.</param>
    /// <returns>The Wrapped Angle</returns>
    public static float AngleWrapDegrees(float baseAngle, float angleToBeWrapped) {
        while (baseAngle > angleToBeWrapped + 180) angleToBeWrapped += 360;
        while (baseAngle < angleToBeWrapped - 180) angleToBeWrapped -= 360;

        return angleToBeWrapped;
    }

    // If it's wrapped around the sphere too far, we want to wrap it 
    public static Quaternion WrapQuaternion(Quaternion baseAngle, Quaternion angleToBeWrapped)  {
        
        float alignment = Quaternion.Dot(baseAngle, angleToBeWrapped); // 1 = same rotation, -1 means opposite rotation, 0 is perpendicular

        if (alignment < 0) {

            angleToBeWrapped.x *= -1;
            angleToBeWrapped.y *= -1;
            angleToBeWrapped.z *= -1;
            angleToBeWrapped.w *= -1;

        }

        return angleToBeWrapped; // No change if the angle is within 180 degrees
    }
}

