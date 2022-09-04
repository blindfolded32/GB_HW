using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TaskOperator : MonoBehaviour
    {
        public Button TaskStartButton;
        public Button TaskStopButton;
        public Text TimeFromStartValue;
        public Text UpdatesFromStartValue;
        public bool _isTasksStarted;
        CancellationTokenSource _cancelTokenSource;
        CancellationToken _cancelToken;

        private void Start()
        {
            Application.targetFrameRate = 60;
            TimeFromStartValue.text = "0.0";
            UpdatesFromStartValue.text = "0";
            TaskStartButton.onClick.AddListener(StartOperation);
            TaskStopButton.onClick.AddListener(Cancel);
            TaskStopButton.interactable = false;
        }

        public void StartOperation()
        {
            if (_isTasksStarted) return;
            _isTasksStarted = true;
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;

            RunTasksAsync(_cancelToken);
        }

        private async void RunTasksAsync(CancellationToken cancelToken)
        {
            TaskStopButton.interactable = true;
            TimeFromStartValue.text = "0.0";
            UpdatesFromStartValue.text = "0";
            var task1 = TimeLogAsync(cancelToken);
            var task2 = UpdateLogAsync(cancelToken);
            var resultTask = await Task.WhenAny(task1, task2);

            Debug.Log("Result: " + resultTask.Result.ToString());
            _isTasksStarted = false;
            _cancelTokenSource.Cancel(); // stop other tasks
            _cancelTokenSource.Dispose();
            TaskStopButton.interactable = false;
        }

        public void Cancel()
        {
           _cancelTokenSource.Cancel();
        }

        private async Task<bool> TimeLogAsync(CancellationToken cancelToken)
        {
            var time = 0f;

            while (time <= 1.0f) 
            {
                if(cancelToken.IsCancellationRequested)
                {
                    Debug.Log("Time spent: " + time.ToString());
                    return false;
                }

                await Task.Yield();
                time += Time.deltaTime;
                TimeFromStartValue.text = time.ToString();
            }

            return true;
        }

        private async Task<bool> UpdateLogAsync(CancellationToken cancelToken)
        {
            var updates = 0;
            while (updates < 59) 
            {
                if (cancelToken.IsCancellationRequested)
                {
                    Debug.Log("Updates done: " + updates.ToString());
                    return false;
                }

                await Task.Yield();
                updates++;
                UpdatesFromStartValue.text = updates.ToString();
            }
        
            return false;
        }

        private void OnDestroy()
        {
            TaskStartButton.onClick.RemoveAllListeners();
            TaskStopButton.onClick.RemoveAllListeners();
            if (_cancelTokenSource != null) _cancelTokenSource.Dispose();
        }
    }
}