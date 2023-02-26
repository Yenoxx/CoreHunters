using System;
using UnityEngine;

public class Clock
{
    private float time_;
    public float time { 
        get
        {
            return time_;
        }
        set
        {
            elapsed = value;
            time_ = value;
        }
    }

    public float elapsed { get; private set;}
    public bool active { get; set; }
    public bool restart { get; set; }

    public Action tick;

    public Clock(float time)
    {
        this.time = time;
        active = false;
        restart = false;
        tick = () => {};
    }
    public Clock() : this(1) {}

    public void Update()
    {
        if (active)
        {
            if (elapsed > 0)
            {
                elapsed -= Time.deltaTime;
            }
            else
            {
                tick.Invoke();
                if (restart) 
                {
                    elapsed += time;
                }
                else 
                {
                    active = false;
                    elapsed = time;
                }
            }
        }
    }

    public void Start()
    {
        elapsed = time;
        active = true;
    }
    public void Start(float time)
    {
        this.time_ = time;
        Start();
    }

    public void Stop()
    {
        elapsed = time;
        active = false;
    }
}