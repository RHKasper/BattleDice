using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace BattleTest.Scripts
{
    public static class UserCueSequencer
    {
        private static readonly Queue<Cue> _queueOfCues = new();

        public static void EnqueueCue(Cue cue)
        {
            _queueOfCues.Enqueue(cue);
        }
        
        public static void EnqueueCue(Func<Task> cue)
        {
            _queueOfCues.Enqueue(new Cue(cue, AlwaysTrue));
        }

        public static void EnqueueCue(GameObject requiredGameObject, Func<Task> cue)
        {
            _queueOfCues.Enqueue(new Cue(cue, () => requiredGameObject));
        }
        
        public static void ClearQueuedCues()
        {
            _queueOfCues.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static async void ProcessQueue()
        {
            while (true)
            {
                Cue cue = DequeueToNextValidCue();
                if (cue != null)
                {
                    await cue.AsyncAction();
                }
                else
                {
                    await Task.Delay(50);
                }
            }
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