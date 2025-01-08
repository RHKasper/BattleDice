using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace BattleTest.Scripts
{
    // todo: cleanup surface methods for this class
    public static class UserCueSequencer
    {
        public const int DefaultCueDelayMs = 100;
        
        private static readonly Queue<Cue> _queueOfCues = new();
        private static bool _alive = true;
        private static int _cueInterval = 16;

        public static void EnqueueCue(Cue cue)
        {
            _queueOfCues.Enqueue(cue);
        }

        public static void EnqueueCueWithDelayAfter(GameObject requiredGameObject, Func<Task> action, int delayMs = DefaultCueDelayMs)
        {
            Cue cue = new Cue(async () =>
            {
                await action.Invoke();
                await Task.Delay(delayMs);
            }, GenerateGameObjectExistsFunc(requiredGameObject));
            EnqueueCue(cue);
        }
        
        public static void EnqueueCueWithDelayAfter(GameObject requiredGameObject, Action action, int delayMs = DefaultCueDelayMs)
        {
            Cue cue = new Cue(async () =>
            {
                action.Invoke();
                await Task.Delay(delayMs);
            }, GenerateGameObjectExistsFunc(requiredGameObject));
            EnqueueCue(cue);
        }
        
        public static void EnqueueCueWithDelayAfter(Action action, int delayMs = DefaultCueDelayMs)
        {
            Cue cue = new Cue(async () =>
            {
                action.Invoke();
                await Task.Delay(delayMs);
            }, AlwaysTrue);
            EnqueueCue(cue);
        }
        
        public static void EnqueueCue(Func<Task> cue)
        {
            _queueOfCues.Enqueue(new Cue(cue, AlwaysTrue));
        }
        
        public static void EnqueueCue(Action cue)
        {
            _queueOfCues.Enqueue(new Cue(() =>
            {
                cue.Invoke();
                return Task.CompletedTask;
            }, AlwaysTrue));
        }

        public static void EnqueueCue(GameObject requiredGameObject, Func<Task> cue)
        {
            _queueOfCues.Enqueue(new Cue(cue, GenerateGameObjectExistsFunc(requiredGameObject)));
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
                    await cue.AsyncAction();
                }
                else
                {
                    await Task.Delay(_cueInterval);
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
            public Func<Task> AsyncAction;
            public Func<bool> IsValid;

            public Cue(Func<Task> asyncAction, Func<bool> isValid)
            {
                AsyncAction = asyncAction;
                IsValid = isValid;
            }
        }
    }
}