using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace HW2
{
    public class Part1 : MonoBehaviour
    {

        public Button JobStartButton;
        private NativeArray<int> _inputNativeArray;
        private Part1JobStruct _jobStruct;
        private JobHandle _jobHandle;
        private void Start()
        {
            JobStartButton.onClick.AddListener(ExecuteJob);
        }

        private void ExecuteJob()
        {
             GenerateArray();
             PrintArray();
            _jobStruct = new Part1JobStruct(){InpunNativeArray =  _inputNativeArray};
            _jobHandle = _jobStruct.Schedule(_jobHandle);
            _jobHandle.Complete();
            StartCoroutine(JobCoroutine());

        }
        private void GenerateArray()
        {
            _inputNativeArray = new NativeArray<int>(10,Allocator.Persistent);
            for (var i = 0; i < _inputNativeArray.Length; i++)
            {
                _inputNativeArray[i] = Random.Range(0, 100);
            }
        }

        private IEnumerator JobCoroutine()
        {
            while (_jobHandle.IsCompleted == false)
            {
                yield return new WaitForEndOfFrame();
            }
            PrintArray();
            _inputNativeArray.Dispose();
        }
        private void PrintArray()
        {
            string output =  "Generated array element ";
            for (var i = 0; i < _inputNativeArray.Length; i++)
            {
                output += $"{_inputNativeArray[i]}, ";
            }
            Debug.Log(output);
        }

        private void OnDestroy()
        {
           if(_inputNativeArray.IsCreated) _inputNativeArray.Dispose();
            JobStartButton.onClick.RemoveAllListeners();
        }
    }

    public struct Part1JobStruct : IJob
    {

        public NativeArray<int> InpunNativeArray;
        
        public void Execute()
        {
            for (int i = 0; i < InpunNativeArray.Length; i++)
            {
                if (InpunNativeArray[i] >= 10) InpunNativeArray[i] = 0;
            }
        }
    }
}