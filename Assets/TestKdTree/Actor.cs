using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Comp
{
    White,
    Black,
}

public class Actor : MonoBehaviour
{
    public delegate void OnDead(Actor actor);
    public event OnDead onDead;

    public Comp comp;
    public Transform m_transform;
    void Awake()
    {
        var rP = Random.insideUnitCircle * 50f;
        m_transform = this.transform;
        m_transform.position = new Vector3(rP.x, 0, rP.y);
    }


    private void Update()
    {
        var r1 = Random.Range(0, 10000);

        if (r1 < 50)
        {
            var rP = Random.insideUnitCircle * 50f;
            m_transform.position = new Vector3(rP.x, 0, rP.y);
        }
    }

    private void OnDestroy()
    {
        if (onDead != null)
        {
            onDead(this);
        }
    }
}
