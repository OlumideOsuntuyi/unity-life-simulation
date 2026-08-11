using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    static MainThreadDispatcher Instance;
    Queue<Action> MAIN_THREAD_QUEUE = new();
    Queue<Action> ALT_THREAD_QUEUE = new();
    Thread queueThread;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        queueThread = new Thread(new ThreadStart(AltUpdate));
        queueThread.Start();
    }
    public static void QueueMain(Action action)
    {
        Instance.Queue(action, true);
    }
    public static void QueueAlt(Action action)
    {
        Instance.Queue(action, false);
    }
    public void Queue(Action action, bool mainThread)
    {
        if(mainThread)
        {
            MAIN_THREAD_QUEUE.Enqueue(action);
        }
        else
        {
            ALT_THREAD_QUEUE.Enqueue(action);
        }
    }
    private void Update()
    {
        if (MAIN_THREAD_QUEUE.Count > 0)
        {
            MAIN_THREAD_QUEUE.Dequeue()?.Invoke();
        }
    }
    private void AltUpdate()
    {
        while (true)
        {
            if(ALT_THREAD_QUEUE.Count > 0)
            {
                ALT_THREAD_QUEUE.Dequeue()?.Invoke();
            }
        }
    }
    private void OnDestroy()
    {
        queueThread?.Abort();
    }
}