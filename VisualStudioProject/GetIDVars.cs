using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace XimiaEGE
{
    public static class Setting
    {
        public const string Cookie = "__cfduid=d344ea38fa160302641f91690df795e9f1569144468; pysid=95247ce9b3ea4e7eeb91b08f4506351c; top100_id=t1.2487439.1567562986.1569144413399; _ym_uid=156914441449964207; _ym_d=1569144414; _ga=GA1.2.2019260599.1569144414; last_visit=1575635988997::1575646788997; __gads=Test; _ym_isad=2; _gid=GA1.2.1281781142.1575629750; cookie_accepted=yes; tmr_detect=0%7C1575646796453; _ym_visorc_51144176=b";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0";
        public const string AcceptLanguage = "ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3";
        public const string Url = "https://chem-ege.sdamgia.ru";
        public const string NameFolderData = "Data";
        public const string NameJsonBd = "BD.json";
      //  public const string PatchExampleHtmlFile = "Example.html";
    }
    public static class GetMonths
    {
        public const string Main = @"";
        public const string Archive = @"/archive";
        /// <summary>
        /// Получение информации о всех месяцах и id вариантов в них
        /// </summary>
        /// <param name="Url">Базовая ссылка по умолчанию установлена ссылка на химию</param>
        /// <returns></returns>
        public static List<MonthId> Get()
        {
            List<MonthId> months = new List<MonthId>(0);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Function.getHTML(Setting.Url));
            // Парсинг главной страницы 
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//tr");
            MonthId month = new MonthId { Mesac = ("New " + ((Monthes)(DateTime.Now.Month)).ToString()), varXimIndices = new List<Var>(0) };
            foreach (var Component in htmlNodes)
            {
                try
                {
                    if (Component.Attributes["class"].Value == "pinkmark")
                    {
                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(Component.InnerHtml);
                        HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//a");
                        foreach (var ComponentTwo in nodes)
                        {
                            try
                            {
                                month.varXimIndices.Add(new Var
                                {
                                    Id = ComponentTwo.Attributes["href"].Value.Replace("/test?id=", ""),
                                    Num = (uint)(month.varXimIndices.Count + 1)
                                });
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
            months.Add(month);
            // Парсинг всего архива
            htmlDocument.LoadHtml(Function.getHTML(Setting.Url + Archive));
            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div");
            if (htmlNodes != null)
            {
                foreach (var Component in htmlNodes)
                {
                    try
                    {
                        if (Component.Attributes["class"].Value == "content")
                        {   //поиск месяцов и их контентов
                            List<string> Mesaca = new List<string>(0);
                            List<HtmlDocument> HtmlMesacContent = new List<HtmlDocument>(0);
                            htmlDocument.LoadHtml(Component.InnerHtml);
                            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//h3");
                            //Записываем месяца согласно их порядку
                            foreach (var ComponentTmo in htmlNodes)
                            {
                                try
                                {
                                    if (ComponentTmo.Attributes["style"].Value == "margin-bottom:-5px")
                                    {
                                        Mesaca.Add("Архив: " + ComponentTmo.InnerText);
                                    }
                                }
                                catch { }
                            }
                            //Записываем контент каждого месяца
                            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//table");
                            foreach (var ComponentTmo in htmlNodes)
                            {
                                try
                                {
                                    if (ComponentTmo.Attributes["class"].Value == "our_test")
                                    {
                                        HtmlDocument html = new HtmlDocument();
                                        html.LoadHtml(ComponentTmo.InnerHtml);
                                        HtmlMesacContent.Add(html);
                                    }
                                }
                                catch { }
                            }
                            //Добавление данных о каждом месяце и их вариантах
                            for (int i = 0; i < HtmlMesacContent.Count; i++)
                            {
                                htmlNodes = HtmlMesacContent[i].DocumentNode.SelectNodes("//a");
                                List<Var> IdVar = new List<Var>(0);
                                foreach (var ComponentTwo in htmlNodes)
                                {
                                    try
                                    {
                                        IdVar.Add(new Var { Num = (uint)(IdVar.Count + 1), Id = ComponentTwo.Attributes["href"].Value.Remove(0, ComponentTwo.Attributes["href"].Value.LastIndexOf('=') + 1) });
                                    }
                                    catch { }
                                }
                                months.Add(new MonthId { Mesac = Mesaca[i], varXimIndices = IdVar });
                            }
                            break;
                        }
                    }
                    catch { }
                }
            }
            return months;
        }

        /// <summary>
        /// Все возможные вариации названий месяца
        /// </summary>
        private enum Monthes
        {
            ЯНВАРСКИЕ = 1,
            ФЕВРАЛЬСКИЕ = 2,
            МАРТОВСКИЕ = 3,
            АПРЕЛЬСКИЕ = 4,
            МАЙСКИЕ = 5,
            ИЮНЬСКИЕ = 6,
            ИЮЛЬСКИЕ = 7,
            АВГУСТОВСКИЕ = 8,
            СЕНТЯБРЬСКИЕ = 9,
            ОКТЯБРЬСКИЕ = 10,
            НОЯБРЬСКИЕ = 11,
            ДЕКАБРЬСКИЕ = 12
        }
        /// <summary>
        /// Данные одного варианта
        /// </summary>
        public struct Var
        {
            /// <summary>
            /// ID варианта
            /// </summary>
            public string Id;
            /// <summary>
            /// Номер варианта
            /// </summary>
            public uint Num;
        }
        /// <summary>
        /// Информация о варианте 
        /// его название и id вариантов
        /// </summary>
        public struct MonthId
        {
            /// <summary>
            /// Месяц
            /// </summary>
            public string Mesac;
            /// <summary>
            /// Список вариантов
            /// </summary>
            public List<Var> varXimIndices;
        }
    }
    public static class GetVar
    {

        public static VarOtvet Get(string ID, string Patch, bool C_Part = true, MainF.Messenge messenge = null)
        {
            DatVarHTML datVarHTML = GetDataVar(ID);
            VarOtvet varOtvet = new VarOtvet { Id = ID, otvet_A_Parts = new List<Otvet_A_Part>(0), otvet_C_Parts = new List<Otvet_C_Part>(0) };
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Function.PostHtml(Setting.Url + @"/test", datVarHTML.Data));
            // Получаем Html двух таблиц
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div");
            string OneTab = null;
            string TwoTab = null;

            foreach (var Elem in htmlNodes)
            {
                try
                {
                    if (Elem.Attributes["style"].Value == "vertical-align:top;margin: 5px;text-align:center; display:inline-block; max-width:300px")
                        TwoTab = Elem.InnerHtml;
                    else if (Elem.Attributes["style"].Value == "vertical-align:top;margin: 5px;text-align:center; display:inline-block; max-width:500px")
                        OneTab = Elem.InnerHtml;
                    else if (OneTab != null && TwoTab != null)
                        break;
                }
                catch { }
            }
            // Парсинг части A
            htmlDocument.LoadHtml(OneTab);
            htmlNodes = htmlDocument.DocumentNode.SelectNodes("//tr");
            foreach (var Component in htmlNodes)
            {
                try
                {
                    if (Component.Attributes["class"].Value == "res_row")
                    {
                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(Component.InnerHtml);
                        HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//td");
                        if (nodes != null)
                        {
                            varOtvet.otvet_A_Parts.Add(new Otvet_A_Part { Num = (uint)(varOtvet.otvet_A_Parts.Count + 1), Resul = nodes[nodes.Count - 1].InnerText });
                        }
                    }
                }
                catch { }
            }
            // Парсинг части C
            Patch += "\\" + varOtvet.Id;
            Directory.CreateDirectory(Patch);
            if (C_Part)
            {
                htmlDocument.LoadHtml(TwoTab);
                htmlNodes = htmlDocument.DocumentNode.SelectNodes("//tr");
                foreach (var Component in htmlNodes)
                {
                    if (Component.Attributes["class"]?.Value == "res_row")
                    {
                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(Component.InnerHtml);
                        HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//td");
                        if (nodes != null)
                        {
                            var Res = Get_C_Part_OneNum(nodes[1].InnerText, Patch, messenge);
                            Res.Num = (uint)(varOtvet.otvet_C_Parts.Count + 1);
                            varOtvet.otvet_C_Parts.Add(Res);
                        }
                    }
                }
            }
            return varOtvet;
        }
        private static Otvet_C_Part Get_C_Part_OneNum(string IdNum, string Patch, MainF.Messenge messenge)
        {
            Otvet_C_Part otvet = new Otvet_C_Part();
            Patch += "\\" + IdNum;
            Directory.CreateDirectory(Patch);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Function.getHTML(Setting.Url + @"/problem?id=" + IdNum));
            // Поиск ответа на странице
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div");
            foreach (var Elem in htmlNodes)
            {
                try
                {
                    if (Elem.Attributes["id"].Value == "sol" + IdNum)
                    {
                        htmlDocument.LoadHtml(Elem.InnerHtml);
                        otvet.Data = Elem.InnerHtml;
                        break;
                    }
                }
                catch { }
            }
            // Парсин элементов ответа
            if (otvet.Data != null)
            {
                htmlNodes = htmlDocument.DocumentNode.SelectNodes("//img");
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", Setting.UserAgent);
                webClient.Headers.Add("Cookie", Setting.Cookie);
                foreach (var Elem in htmlNodes)
                {
                restartImg:
                    try
                    {
                        string PatchFileSave = Elem.Attributes["src"].Value;
                        PatchFileSave = PatchFileSave.Remove(0, PatchFileSave.LastIndexOf("/") + 1);
                        PatchFileSave = PatchFileSave.Replace('?', '1').Replace('=', '9');
                        PatchFileSave = Patch + "\\" + PatchFileSave;
                        PatchFileSave += PatchFileSave.Contains(".svg") ? null : ".svg";
                        string UrlDownload = Elem.Attributes["src"].Value.Contains("http") ? Elem.Attributes["src"].Value : (Setting.Url + Elem.Attributes["src"].Value);
                        webClient.DownloadFile(UrlDownload, PatchFileSave);
                        otvet.Data = otvet.Data.Replace(Elem.Attributes["src"].Value, @"file:///" + PatchFileSave);
                    }
                    catch
                    {
                        goto restartImg;
                    }

                }
            }
            else
            {
                Console.WriteLine("Error");
            }

            return otvet;
        }
        /// <summary>
        /// Получение данных варианта (все номера)
        /// </summary>
        /// <param name="IDVar">Номер варианта</param>
        /// <returns>Полученные данные</returns>
        private static DatVarHTML GetDataVar(string IDVar)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            string HTML = Function.getHTML(@"https://chem-ege.sdamgia.ru/test?id=" + IDVar);
            htmlDocument.LoadHtml(HTML);
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//input");
            DatVarHTML datVarHTML = new DatVarHTML();
            datVarHTML.Data = @"timer=153&stat_id=0&test_id=" + IDVar;
            foreach (var Comp in htmlNodes)
            {
                try
                {
                    if (Comp.Attributes["class"].Value == "test_inp" && Comp.Attributes["name"].Value.Contains("answer"))
                    {
                        datVarHTML.Data += "&" + Comp.Attributes["name"].Value + "=";
                        if (Comp.Attributes["name"].Value.Contains('C') || Comp.Attributes["name"].Value.Contains('c'))
                        {
                            datVarHTML.C_part++;
                        }
                        else
                        {
                            datVarHTML.A_Part++;
                        }
                    }
                }
                catch { }

            }
            datVarHTML.Data = datVarHTML.Data.Replace('c', 'C');
            datVarHTML.Data += "&a=check&is_cr=1%22";
            return datVarHTML;
        }
        /// <summary>
        /// Информация о номерах на странице
        /// </summary>
        private struct DatVarHTML
        {
            public string Data;
            public int A_Part;
            public int C_part;
        }
        /// <summary>
        /// Ответы на вариант
        /// </summary>
        [System.Serializable]
        public struct VarOtvet
        {
            /// <summary>
            /// ID Варианта
            /// </summary>
            public string Id;
            /// <summary>
            /// Ответы на A часть
            /// </summary>
            public List<Otvet_A_Part> otvet_A_Parts;
            /// <summary>
            /// Ответы на C часть
            /// </summary>
            public List<Otvet_C_Part> otvet_C_Parts;
        }
        [System.Serializable]
        public struct Otvet_A_Part
        {
            public uint Num;
            public string Resul;
        }
        [System.Serializable]
        public struct Otvet_C_Part
        {
            public uint Num;
            public string Data;
        }
    }
    static class Function
    {
        /// <summary>
        /// Получение HTML страницы
        /// </summary>
        /// <param name="url">Ссылка на сайт</param>
        /// <returns></returns>
        public static string getHTML(string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, Setting.AcceptLanguage);
                httpWebRequest.UserAgent = Setting.UserAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Cookie", Setting.Cookie);
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var stream = httpWebResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static string PostHtml(string is_card_URL, string Data)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(is_card_URL);
                httpWebRequest.Headers.Add("Cookie", Setting.Cookie);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                Encoding encoding = Encoding.UTF8;
                byte[] byte1 = encoding.GetBytes(Data);
                httpWebRequest.ContentLength = byte1.Length;
                Stream st = httpWebRequest.GetRequestStream();
                st.Write(byte1, 0, byte1.Length);
                st.Close();
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }
    public static class MainF
    {
        public delegate void Messenge(string Mess);

        public static List<Month> monthsGlob = new List<Month>(0);
        public static void DownloadBD(string PatchSave = null, Messenge messenge = null)
        {
            PatchSave += PatchSave != null ? PatchSave + "\\" + Setting.NameFolderData : Setting.NameFolderData;
            Directory.CreateDirectory(PatchSave);
            List<Month> months = new List<Month>(0);
        // Получение списка месяцев
        restart: try
            {
                var Mon = GetMonths.Get();
                if (messenge != null) messenge.Invoke("Будет загруженно " + Mon.Count + " вариантов");
                // Проход по месяцам
                foreach (var elem in Mon)
                {
                    Month month = new Month { Name = elem.Mesac, vars = new List<Var>(0) };
                    // Проход по вариантам
                    string PatchVar = PatchSave + "\\" + elem.Mesac.Replace(":", null).Replace(" ", null);
                    PatchVar = Path.GetFullPath(PatchVar);
                    Directory.CreateDirectory(PatchVar);
                    foreach (var ElemTwo in elem.varXimIndices)
                    {
                        if (messenge != null) messenge.Invoke("Начинаю загрузку: " + ElemTwo.Num);
                        res:
                        try
                        {
                            month.vars.Add(new Var { Num = ElemTwo.Num, varOtvet = GetVar.Get(ElemTwo.Id, PatchVar, true, messenge) });
                        }
                        catch { if (messenge != null) messenge.Invoke("Произошла ошибка осуществляю перезапуск: " + ElemTwo.Num); goto res; }
                        if (messenge != null) messenge.Invoke("Скачал вариант: " + ElemTwo.Num);
                    }
                    if (messenge != null) messenge.Invoke("Скачал месяц: " + elem.Mesac);
                    months.Add(month);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); goto restart; }
            monthsGlob = months;
            File.WriteAllText((PatchSave != null) ? PatchSave + "\\" + Setting.NameJsonBd : Setting.NameJsonBd, JsonConvert.SerializeObject(months));
            if (messenge != null) messenge.Invoke("Готово");
        }
        [System.Serializable]
        public struct Month
        {
            public string Name;
            public List<Var> vars;
        }
        [System.Serializable]
        public struct Var
        {
            public uint Num;
            public GetVar.VarOtvet varOtvet;
        }
        public static bool LoadData(string Patch = null)
        {
            Patch = Patch != null ? Patch + "\\" + Setting.NameFolderData : Setting.NameFolderData;
            if (!File.Exists((Patch != null) ? Patch + "\\" + Setting.NameJsonBd : Setting.NameJsonBd))
                return false;
            monthsGlob = JsonConvert.DeserializeObject<List<Month>>(File.ReadAllText((Patch != null) ? Patch + "\\" + Setting.NameJsonBd : Setting.NameJsonBd));
            return true;
        }
        public static ResulDataVarEcho GetVarID(string Id, string Patch, bool DownloadAutoVar = true)
        {
            // Поиск среди бд
            bool state = true;
            foreach (var Mon in monthsGlob)
            {
                foreach (var v in Mon.vars)
                {
                    if (v.varOtvet.Id == Id)
                    {
                        state = false;
                        return new ResulDataVarEcho { NameMesac = Mon.Name, var = v };
                    }
                }
                if (!state) break;
            }
            // В бд не удалось найти
            if (DownloadAutoVar)
                return new ResulDataVarEcho { NameMesac = "NotName", var = new Var { Num = uint.MaxValue, varOtvet = GetVar.Get(Id, Patch, false) } };
            return new ResulDataVarEcho { NameMesac = "NotDownloadVar" }; // 999 Говорит о не найденном варианте
        }

        public static string GetHtmlReshenie(List<uint> Nums, string[] Vars, string Patch)
        {
            string PatchHtml = "Test.html";
            ResulDataVarEcho[] vars = new ResulDataVarEcho[Vars.Length];
            // 
            for (int i = 0; i < Vars.Length; i++)
                vars[i] = GetVarID(Vars[i], Patch, false);
            foreach (var Elem in vars)
                if (Elem.NameMesac == "NotDownloadVar")
                    return GetHtmlReshenieDownload();
            File.WriteAllText("sds.html", GenerateStaticHtml(vars, Nums));
            return PatchHtml;
        }
        private static string GetHtmlReshenieDownload()
        {
            return null;
        }
        private static string GenerateStaticHtml(ResulDataVarEcho[] varEchoes, List<uint> Nums)
        {
            uint LinkCPart = 0;
            string Resul = "";
            foreach (var Elem in varEchoes)
                Resul += GenerateHtmlVar(Elem, Nums, ref LinkCPart);
            return HtmlExempleGet.Htlml.Replace("{0}", varEchoes[0].NameMesac).Replace("{1}", Resul);
        }
        private static string GenerateHtmlVar(ResulDataVarEcho var, List<uint> Nums, ref uint LinkCPart)
        {
            return "<div class =\"DataVar\" id = \"" + var.var.varOtvet.Id + "\"><h2><b>" + "Вариант номер: " + var.var.Num + "      " + ((var.NameMesac != null && var.NameMesac != "NotDownloadVar" && var.NameMesac != "NotName") ? "Месяц: " + var.NameMesac : null) + " ID [" + var.var.varOtvet.Id + "]" + "</b></h2>" + GenerateTap_Part_A(var, Nums) + GenerateTap_Part_C(var, Nums, ref LinkCPart) + "</div>";
        }
        private static string GenerateTap_Part_A(ResulDataVarEcho var, List<uint> Nums)
        {
            string resul = "<div class=\"Tab_A_Part\"><table>";
            foreach (var Elem in var.var.varOtvet.otvet_A_Parts)
                if (Nums.Contains(Elem.Num)) resul += "<tr><td>Номер " + Elem.Num + "</td><td>" + Elem.Resul + "</td></tr>";
            return resul + "</table></div>";
        }
        private static string GenerateTap_Part_C(ResulDataVarEcho var, List<uint> Nums, ref uint LinkCPart)
        {
            LinkCPart++;
            string resul = null;
            foreach (var Elem in var.var.varOtvet.otvet_C_Parts)
                if (Nums.Contains(Elem.Num + (uint)var.var.varOtvet.otvet_A_Parts.Count)) resul += "<div class = \"Num_C_Part\" id = \"" + Elem.Num+ "\"><h2><b> Задание номер:" + (Elem.Num + (uint)var.var.varOtvet.otvet_A_Parts.Count) + "</b></h2>" + Elem.Data+"</div>";
            if(resul!=null)
                return "<input type=\"checkbox\" id=\"hd - " + LinkCPart + "\" class=\"hide\"/><label for=\"hd - " + LinkCPart + "\" >Скрыть & Раскрыть</label><div class=\"Tab_C_Part\">" + resul + "</div>";
            return null;
        }
        public struct ResulDataVarEcho
        {
            public string NameMesac;
            public Var var;
        }
    }
}