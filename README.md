# Ximia_ege.sdamgia
 Парс сайта решу ЕГЭ и создание html с ответами

Весь необходимый набор для взаимодействия лежит в `XimiaEGE.MainF`:
* Парс данных (Download) [с решу егэ](https://chem-ege.sdamgia.ru) - `DownloadBD(string PatchSave = null, Messenge messenge = null);`
  * <b>PatchSave</b> - Путь к директории сохранения
  * <b>messenge</b> - Делегат для метода `void Messenge(string Mess)`, где `string Mess` сообщение (данная метод используется для логирования состояния)
* Загрузка (Load) Бд `LoadData(string Patch = null, bool UpdaeData = false);`
  * <b>Возвращает</b> тип bool True - успешно подгруженна бд, False - загрузка не удалась
  * <b>Patch</b> - Путь к директории с бд
  * <b>UpdaeData</b> - <b>В разработке!!!</b> на данный момент параметр не трогать, только увеличивает время загрузки бд
* Получить вариант `GetVarID(string Id, string Patch, bool DownloadAutoVar = true);`
  * <b>Возвращает</b> тип ResulDataVarEcho в себе который содержит:
    * `string NameMesac` - Название месяца
    * `Var var` - Данный тип предназначен для хранения данные месяца содержит в себе:
      * `uint Num` - Личный индификатор месяца
      * `GetVar.VarOtvet varOtvet` - Данный тип предназначен для хранения ответов как на A, так и на C часть и содержит в себе:
        * 
