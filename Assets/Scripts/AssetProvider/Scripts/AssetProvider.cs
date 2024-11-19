using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetProvider.Scripts
{
  /// \class AssetProvider
  /// \brief Класс поставщик ресурсов
  public sealed class AssetProvider : IAssetProvider
  {
    /// \brief Коллекция завершенных операций загрузки
    private readonly Dictionary<string, AsyncOperationHandle> _completeCache = new();
    /// \brief Коллекция струтур-доступов к завершенным операциям загрузки
    private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
    
    /// \brief Метод загрузки ресурса через Addressables
    /// \param address Адресс указывающий на объект ресурса, который нужно загрузить в память
    /// \param T - тип ресурса
    /// \return UniTask который можно подождать до завершения процесса загрузки ресурса
    public async UniTask<T> Load<T>(string address) where T : class
    {
      if (_completeCache.TryGetValue(address, out AsyncOperationHandle completedHandle))
        return completedHandle.Result as T;

      return await RunWithCacheOnComplete(
        Addressables.LoadAssetAsync<T>(address),
        address);
    }
    
    /// \brief Метод выгрузки ресурса через Addressables
    /// \param address Адресс указывающий на объект ресурса, который нужно выгрузить из памяти
    public void Release(string address)
    {
      if (!_handles.ContainsKey(address))
        return;

      foreach (AsyncOperationHandle handle in _handles[address])
        Addressables.Release(handle);

      if (_handles.ContainsKey(address))
        _handles.Remove(address);

      if (_completeCache.ContainsKey(address))
        _completeCache.Remove(address);
    }

    /// \brief Метод добавления доступа к операции в коллекцию
    /// \param key Адресс указывающий на объект ресурса
    /// \param handle Структура-доступ к завершенной операции
    private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
    {
      if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
      {
        resourceHandles = new List<AsyncOperationHandle>();
        _handles[key] = resourceHandles;
      }

      resourceHandles.Add(handle);
    }
    
    /// \brief Метод, вызываемый при завершении операции загрузки ресурса
    /// \param handle Структура-доступ к завершенной операции
    /// \param cacheKey Адресс указывающий на объект ресурса
    /// \return UniTask, который можно подождать до завершения процесса загрузки ресурса
    private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
    {
      handle.Completed += completeHandle => _completeCache[cacheKey] = completeHandle;

      AddHandle(cacheKey, handle);

      return await handle.Task;
    }
  }
}