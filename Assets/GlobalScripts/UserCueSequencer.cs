using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace GlobalScripts
{
    // todo: cleanup surface methods for this class
    public static class UserCueSequencer
    {
        public const int DefaultCueDelayMs = 100;
        
        private static readonly Queue<Cue> _queueOfCues = new();
        private static bool _alive = true;

        public static void EnqueueCue(Cue cue)
        {
            _queueOfCues.Enqueue(cue);
        }
        
        public static void EnqueueCueWithDelayAfter(GameObject requiredGameObject, Func<Task> action, string cueName)
        {
            Cue cue = new Cue(cueName, async () =>
            {
                await action.Invoke();
                await WebGlUtil.WebGlSafeDelay(DefaultCueDelayMs);
            }, GenerateGameObjectExistsFunc(requiredGameObject));
            EnqueueCue(cue);
        }
        
        public static void EnqueueCueWithDelayAfter(Action action, string cueName)
        {
            Cue cue = new Cue(cueName, async () =>
            {
                action.Invoke();
                await WebGlUtil.WebGlSafeDelay(DefaultCueDelayMs);
            }, AlwaysTrue);
            EnqueueCue(cue);
        }
        
        public static void ClearQueuedCues()
        {
            _queueOfCues.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static async void ProcessQueue()
        {
            Debug.Log("Starting Cue Sequencer");
            Application.quitting += OnApplicationQuitting;
            
            while (_alive)
            {
                Cue cue = DequeueToNextValidCue();
                if (cue != null)
                {
                    Debug.Log("Proceessing Cue: " + cue.Name);
                    await cue.AsyncAction();
                }
                else
                {
                    await Task.Yield();
                }
            }
            
            Debug.Log("Stopping Cue Sequencer");
            Application.quitting -= OnApplicationQuitting;
        }

        private static void OnApplicationQuitting()
        {
            _alive = false;
            
        }

        [CanBeNull]
        private static Cue DequeueToNextValidCue()
        {
            while (_queueOfCues.Count > 0)
            {
                var cue = _queueOfCues.Dequeue();
                if (cue.IsValid())
                {
                    return cue;
                }
            }

            return null;
        }

        private static Func<bool> AlwaysTrue = () => true;
        private static Func<bool> GenerateGameObjectExistsFunc(GameObject g) => () => g;
        
        public class Cue
        {
            public string Name;
            public Func<Task> AsyncAction;
            public Func<bool> IsValid;

            public Cue(string cueName, Func<Task> asyncAction, Func<bool> isValid)
            {
                Name = cueName;
                AsyncAction = asyncAction;
                IsValid = isValid;
            }
        }
    }
}