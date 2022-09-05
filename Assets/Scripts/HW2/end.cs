using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
public class end : MonoBehaviour
{
    
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _count;
    [SerializeField] private float _spawnRadius;
    private NativeArray<int> _angle;
    private NativeArray<Vector3> _directions;
    private TransformAccessArray _accessArray;
    private Material[] _Cubesu;

    private void Start()
    {
        _Cubesu = new Material[_count];
        _angle = new NativeArray<int>(_count, Allocator.Persistent);
       _directions = new NativeArray<Vector3>(_count, Allocator.Persistent);
       for (int i = 0; i < _directions.Length; i++)
       {
           _directions[i] = Random.rotation.eulerAngles.normalized;
           _angle[i] = Random.Range(0, 180);
       }
        _accessArray = new TransformAccessArray(SpawnObj(_prefab, _count, _spawnRadius));
    }
    private void OnDestroy()
    {
        if (_accessArray.isCreated)
        { 
            _accessArray.Dispose();
            _angle.Dispose();
            _directions.Dispose();
        }
    }

    private void Update()
    {
        MyJobParTransform myJobParTransform = new MyJobParTransform()
        {
            angles=_angle,
            DeltaTime =  Time.deltaTime,
            Directions = _directions
        };
        JobHandle jobHandle = myJobParTransform.Schedule(_accessArray);
        jobHandle.Complete();
    }

    Transform[] SpawnObj(GameObject prefab, int count, float spawnRadius)
    {
        Transform[] objects = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            objects[i] = Instantiate(prefab).transform;
            _Cubesu[i] = objects[i].gameObject.GetComponent<MeshRenderer>().material;
            objects[i].position = Random.insideUnitSphere * spawnRadius;
            _Cubesu[i].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        return objects;
    }
}
[BurstCompile]
public struct MyJobParTransform : IJobParallelForTransform
{
    public NativeArray<int> angles;
    public float DeltaTime;
    public NativeArray<Vector3> Directions;
    public void Execute(int index, TransformAccess transform)
    {
        var direction = Directions[index] - transform.position;
        var moveVector = Vector3.Cross(direction, transform.position);
        transform.position += moveVector * DeltaTime;
        transform.localRotation = Quaternion.AngleAxis(angles[index], Vector3.up);
        angles[index] = angles[index] == 180 ? 0 : angles[index]+1;
    }
}