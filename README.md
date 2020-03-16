# Ximia_ege.sdamgia
 Парс сайта решу ЕГЭ и создание html с ответами

Весь необходимый набор для взаимодействия лежит в `XimiaEGE.MainF`:
* Парс данных (Download) [с решу егэ](https://chem-ege.sdamgia.ru) - `DownloadBD(string PatchSave = null, Messenge messenge = null);`
  * <b>PatchSave</b> - Путь к директории сохранения
  * <b>messenge</b> - Делегат для метода `void Messenge(string Mess)`, где `string Mess` сообщение (данная метод используется для логирования состояния)
* Загрузка (Load) Бд `LoadData(string Patch = null, bool UpdaeData = false);`
  * <b>Patch</b> - Путь к директории с бд
  * <b>UpdaeData</b> - <b>В разработке!!!</b> на данный момент параметр не трогать, только увеличивает время загрузки бд
  * <b>Возвращает</b> тип bool True - успешно подгруженна бд, False - загрузка не удалась
* Получить вариант `GetVarID(string Id, string Patch, bool DownloadAutoVar = true);`
  * <b>Id</b> - Id варианта
  * <b>Patch</b> - Путь к месту хранения БД
  * <b>DownloadAutoVar</b> - Если стоит True и в бд не будет запрошенного варианта то он автоматически его скачает
  * <b>Возвращает</b> тип ResulDataVarEcho в себе который содержит:
    * `string NameMesac` - Название месяца
    * `Var var` - Данный тип предназначен для хранения данные месяца содержит в себе:
      * `uint Num` - Номер варианта в месяце (всего их 15)
      * `GetVar.VarOtvet varOtvet` - Данный тип предназначен для хранения ответов как на A, так и на C часть и содержит в себе:
        * `string Id` - ID Варианта
        * `List<Otvet_A_Part> otvet_A_Parts` -  Тип содержащий в себе ответы на A часть, содержит в себе:
          * `Num` - Номер задания среди части A
          * `Resul` - Ответ
        *  `List<Otvet_C_Part> otvet_C_Parts` - Тип содержащий в себе ответы на C часть, содержит в себе:
          * `Num` - Номер задания среди части A
          * `string Data` - Html C части (ссылки на фото ведут на сайт [решу егэ](https://chem-ege.sdamgia.ru))
          * `PatchDat[] PatchFotos` - Тип содержащий в себе данные о местонахождении фотографий, содержит в себе:
            * `string PatchFile` - Путь к загруженному файлу
            * `string Url` - Ссылки на фото ведут на сайт [решу егэ](https://chem-ege.sdamgia.ru)
          
