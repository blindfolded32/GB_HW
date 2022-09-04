using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HW2
{
    public class Part2: MonoBehaviour
    {
        public Button StartParralelJobButton;
        
        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _resultNativeArray;
        private JobHandle _jobHandle;
        

        private void Start()
        {
           
            _jobHandle = new JobHandle();
            StartParralelJobButton.onClick.AddListener(StartParralelJob);
        }

        private void GenerateArrays()
        {
            _resultNativeArray = new NativeArray<Vector3>(10,Allocator.Persistent);
            _positions = new NativeArray<Vector3>(10,Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(10,Allocator.Persistent);
            for (int i = 0; i < 10; i++)
            {
                _positions[i] = GenerateValue();
                _velocities[i] = GenerateValue();
            }
        }
        private Vector3 GenerateValue()
        {
            return  new Vector3(Random.Range(0,100),Random.Range(0,100),Random.Range(0,100));
        }

        private void StartParralelJob()
        {
           GenerateArrays();
           PrintArray(_positions);
           PrintArray(_velocities);
           Vector3SummaryStruct JobStruct = new Vector3SummaryStruct()
           {
               Positions =  _positions,
               Velocities = _velocities,
               ResultNativeArray = _resultNativeArray
           };
           _jobHandle = JobStruct.Schedule(_resultNativeArray.Length,0);
           _jobHandle.Complete();
           StartCoroutine(JobWaiter());
        }

        private IEnumerator JobWaiter()
        {
            while (_jobHandle.IsCompleted == false)
            {
                yield return new WaitForEndOfFrame();
            }
            PrintArray(_resultNativeArray);
            _positions.Dispose();
            _velocities.Dispose();
            _resultNativeArray.Dispose();
        }
        private void PrintArray(NativeArray<Vector3> array)
        {
            string output = "Array:";
            for (int i = 0; i < array.Length; i++)
            {
                output += $"{array[i]}, ";
            }
            
            Debug.Log(output);
        }

        private void OnDestroy()
        {
            if (_velocities.IsCreated) _velocities.Dispose();
            if (_positions.IsCreated) _positions.Dispose();
            if (_resultNativeArray.IsCreated) _resultNativeArray.Dispose();
            StartParralelJobButton.onClick.RemoveAllListeners();
        }
    }

    public struct Vector3SummaryStruct : IJobParallelFor
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> ResultNativeArray;
        public void Execute(int index)
        {
            ResultNativeArray[index] = Positions[index] + Velocities[index];
        }
    }
}