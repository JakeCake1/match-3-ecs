using Cysharp.Threading.Tasks;

namespace AssetProvider.Scripts
{  
  /// \interface IAssetProvider
  /// \brief Интерфейс, описывающий методы взаимодействия с поставщиком ресурсов
  public interface IAssetProvider
  { 
    /// \brief Метод загрузки ресурса
    /// \param address Адресс/путь указывающий на объект ресурса, который нужно загрузить в память
    /// \param T - тип ресурса
    /// \return UniTask который можно подождать до завершения процесса загрузки ресурса
    UniTask<T> Load<T>(string address) where T : class;
    /// \brief Метод выгрузки ресурса
    /// \param address Адресс/путь указывающий на объект ресурса, который нужно выгрузить из памяти
    void Release(string address);
  }
}