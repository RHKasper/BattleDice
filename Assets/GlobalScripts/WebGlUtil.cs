using System.Threading.Tasks;
using UnityEngine;

namespace GlobalScripts
{
    public static class WebGlUtil
    {
        public static async Task WebGlSafeDelay(float milliseconds)
        {
            float endTime = Time.time + .001f * milliseconds;
            Debug.Log("Start time: " + Time.time + " | end time: " + endTime);

            while (Time.time < endTime)
            {
                await Task.Yield();
                Debug.Log("Yielding...");
            }
        }
    }
}