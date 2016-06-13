﻿using UnityEngine;
using System.Collections;
using Messages.sensor_msgs;
using System;

public class LaserScanView : MonoBehaviour
{
    
    private float[] distBuffer;
    private GameObject[] pointBuffer;
    private bool changed;

    private GameObject goParent;
    private Transform posParent;
    private uint recycleCount = 0;
    private float lastUpdate;
    private float angMin, angInc, maxRange;
    private uint maxRecycle = 0;
        /*
    {
        get { return goParent == null ? 100 : goParent.gameObject.GetComponent<LaserVisController>().maxRecycle; }
    }
    */
    private float decay
    {
        get { return goParent == null ? 0f : goParent.gameObject.GetComponent<LaserVisController>().Decay_Time; }
    }
    
    private float pointSize
    {
        get { return goParent == null ? 1f : goParent.gameObject.GetComponent<LaserVisController>().pointSize; }
    }

    public delegate void RecycleCallback(GameObject me);
    public event RecycleCallback Recylce;

    public delegate void IDiedCallback(GameObject me);
    public event IDiedCallback IDied;

    public void recycle()
    {
        // gameObject.hideFlags |= HideFlags.HideAndDontSave;
        gameObject.SetActive(false);
        if (Recylce != null)
            Recylce(gameObject);
    }


    internal void expire()
    {
        // gameObject.hideFlags |= HideFlags.HideAndDontSave;
        gameObject.SetActive(false);
        if (IDied != null)
            IDied(gameObject);
        
    }



    public void SetScan(float time, LaserScan msg, GameObject _goParent, Transform _posParent)
    {
        //compare length of distbuffer and msg.ranges
        //recreate distance array
        goParent = _goParent;
        posParent = _posParent;
        recycleCount++;
        gameObject.SetActive(true);
        angMin = msg.angle_min;
        angInc = msg.angle_increment;
        maxRange = msg.range_max;
        lastUpdate = time;
        if (distBuffer == null || distBuffer.Length != msg.ranges.Length)
            distBuffer = new float[msg.ranges.Length];
        Array.Copy(msg.ranges, distBuffer, distBuffer.Length);
        changed = true;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
            #region SHOULD I be Recycled?
            if (decay > 0.0001 && (Time.fixedTime - lastUpdate) > decay)
            {
                recycle();
                return;
            }
            #endregion

            #region SHOULD I DIE?
            if (recycleCount > maxRecycle)
            {
               // expire();
                //return;
            }
            #endregion

            if (changed)
            {
                //show if hidden (this scan was recycled)
                hideFlags &= ~HideFlags.HideAndDontSave;

                #region RESIZE IF NEEDED, ADD+REMOVE SPHERES AS NEEDED
                //resize sphere array if different from distbuffer
                //remath all circles based on distBuffer
                if (pointBuffer != null && pointBuffer.Length != distBuffer.Length)
                {
                    int oldsize = pointBuffer.Length;
                    int newsize = distBuffer.Length;
                    if (oldsize < newsize)
                    {
                        Array.Resize(ref pointBuffer, newsize);
                        for (int i = oldsize; i < newsize; i++)
                        {
                            GameObject newsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            newsphere.transform.SetParent(transform);
                            pointBuffer[i] = newsphere;
                        }
                    }
                    else
                    {
                        for (int i = oldsize; i >= newsize; i--)
                        {
                            pointBuffer[i].transform.SetParent(null);
                            pointBuffer[i] = null;
                        }
                        Array.Resize(ref pointBuffer, newsize);
                    }
                }
                else if (pointBuffer == null)
                {
                    pointBuffer = new GameObject[distBuffer.Length];
                    for (int i = 0; i < distBuffer.Length; i++)
                    {
                        GameObject newsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        newsphere.transform.SetParent(transform);
                        pointBuffer[i] = newsphere;
                    }
                }
                #endregion

                #region FOR ALL SPHERES ALL THE TIME
                for (int i = 0; i < pointBuffer.Length; i++)
                {   
                    if (distBuffer[i] > (maxRange - 1f) + 0.999f || distBuffer[i] < 0.0001f)
                    {
                        pointBuffer[i].SetActive(false);
                        continue;
                    }

                pointBuffer[i].SetActive(true);

                    pointBuffer[i].transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                //TODO: SET THE POSITION for pointBuffer[i] based on distBuffer[i]
                    Vector3 parentPos = posParent.position;
                    pointBuffer[i].transform.position = new Vector3((float)(distBuffer[i] * Math.Sin(angMin + angInc * i)) + parentPos.x , 1F + parentPos.y, (float)(distBuffer[i] * Math.Cos(angMin + angInc * i)) + parentPos.z);
                }
                Quaternion parentRot = posParent.rotation;

                this.transform.rotation = parentRot;
            #endregion
            changed = false;
            }
        
    }
}