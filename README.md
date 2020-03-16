# Ximia_ege.sdamgia
 Парс сайта решу ЕГЭ и создание html с ответами

Весь необходимый набор для взаимодействия лежит в `XimiaEGE.MainF`:
* Загрузка данных - `DownloadBD(string PatchSave = null, Messenge messenge = null);`
  * <b>PatchSave</b> - Путь к директория
  * <b>messenge</b> - Делегат для метода `void Messenge(string Mess)`, где `string Mess` сообщение (данная метод используется для логирования состояния)
