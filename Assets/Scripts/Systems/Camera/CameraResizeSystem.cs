using Components;
using Components.Cell;
using Components.Common;
using Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Camera
{
  public class CameraResizeSystem : IEcsInitSystem
  {
    private readonly UnityEngine.Camera _camera;
    private readonly CameraData _cameraData;

    public CameraResizeSystem(UnityEngine.Camera camera, CameraData cameraData)
    {
      _cameraData = cameraData;
      _camera = camera;
    }

    public void Init(IEcsSystems systems)
    {
      EcsWorld world = systems.GetWorld();
      
      var filter = world.Filter<CellComponent>().Inc<GridPositionComponent>().Inc<CellViewRefComponent>().End();
      var cellViewPool = world.GetPool<CellViewRefComponent>();

      DefineGridCorners(filter, cellViewPool, out Vector2 minGridCorner, out Vector2 maxGridCorner);

      SetCameraCenter(maxGridCorner, minGridCorner);
      SetCameraSize(maxGridCorner, minGridCorner);

      Debug.Log($"Init: {GetType().Name}");
    }

    private void DefineGridCorners(EcsFilter filter, EcsPool<CellViewRefComponent> cellViewPool, out Vector2 minGridCorner, out Vector2 maxGridCorner)
    {
      minGridCorner = Vector2.positiveInfinity;
      maxGridCorner = Vector2.negativeInfinity;
      
      foreach (int entityIndex in filter)
      {
        ref CellViewRefComponent cellViewRef = ref cellViewPool.Get(entityIndex);

        maxGridCorner = Vector2.Max( cellViewRef.CellView.transform.position, maxGridCorner);
        minGridCorner = Vector2.Min( cellViewRef.CellView.transform.position, minGridCorner);
      }
    }

    private void SetCameraCenter(Vector2 maxGridCorner, Vector2 minGridCorner)
    {
      Vector2 newCameraCenter = (maxGridCorner - minGridCorner) / 2;
      newCameraCenter += _cameraData.FieldViewCenterOffset;
      
      _camera.transform.position = new Vector3(newCameraCenter.x, newCameraCenter.y, _camera.transform.position.z);
    }

    private void SetCameraSize(Vector2 maxGridCorner, Vector2 minGridCorner)
    {
      maxGridCorner.x += _cameraData.FieldHorizontalPadding;
      minGridCorner.x -= _cameraData.FieldHorizontalPadding;
      
      _camera.orthographicSize = maxGridCorner.x - minGridCorner.x;
    }
  }
}