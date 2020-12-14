#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace StroyExp
{
    class App : IExternalApplication
    {
        const string AR_RIBBON_TAB = "StroyExp";
        const string AR_RIBBON_PANEL_SYSTEMS = "Системы";
        const string AR_RIBBON_PANEL_ARCHITECTURE = "Архитектура";
        public Result OnStartup(UIControlledApplication a)
        {
            #region Tab
            try
            {
                a.CreateRibbonTab(AR_RIBBON_TAB);
            }
            catch (Exception) { }
            List<RibbonPanel> panels = a.GetRibbonPanels(AR_RIBBON_TAB);
            #endregion
            #region Panel Системы
            //Создание панели
            RibbonPanel panelSys = null;
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == AR_RIBBON_PANEL_SYSTEMS)
                {
                    panelSys = pnl;
                    break;
                }
            }
            if (panelSys == null) panelSys = a.CreateRibbonPanel(AR_RIBBON_TAB, AR_RIBBON_PANEL_SYSTEMS);

            //Кнопка "Назначение параметра СЭ_Имя системы"
            {
                PushButtonData btnCommandSetSENameSysData = new PushButtonData(
                             "СЭ_Имя системы",
                             "СЭ_Имя системы",
                             Assembly.GetExecutingAssembly().Location,
                             "StroyExp.CommandSetSENameSys"
                             )
                {
                    ToolTip = "Назначение параметра СЭ_Имя системы",
                    LongDescription = @"Задает значение для параметра ""СЭ_Имя системы"" в соответствии с параметром ""Описание"" типа инженерной системы",
                };
                PushButton btnCommandSetSENameSys = panelSys.AddItem(btnCommandSetSENameSysData) as PushButton;
                Image btnCommandSetSENameSysImg = Properties.Resources.SetParam;
                btnCommandSetSENameSys.LargeImage = Convert(btnCommandSetSENameSysImg); //new BitmapImage(new Uri(@"/Resources/Icons/SetParam.png"));
                btnCommandSetSENameSys.Enabled = true;
            }
            #endregion
            #region Panel Архитектура
            RibbonPanel panelArch = null;
            foreach (RibbonPanel pnl in panels)
            {
                if (pnl.Name == AR_RIBBON_PANEL_ARCHITECTURE)
                {
                    panelArch = pnl;
                    break;
                }
            }
            if (panelArch == null) panelArch = a.CreateRibbonPanel(AR_RIBBON_PANEL_ARCHITECTURE);

            //Кнопка "Отделка"
            {
                PushButtonData btnDataSetWallFinishRoomParams = new PushButtonData(
                             "Отделка",
                             "Отделка",
                             Assembly.GetExecutingAssembly().Location,
                             "SetWallFinishRoomParams.Command"
                             )
                {
                    ToolTip = "Назначение параметров отделки",
                    LongDescription = @"",
                };
                PushButton btnSetWallFinishRoomParams = panelArch.AddItem(btnDataSetWallFinishRoomParams) as PushButton;
                Image btnSetWallFinishRoomParamsImg = Properties.Resources.SetParam;
                btnCommandSetSENameSys.LargeImage = Convert(btnCommandSetSENameSysImg); //new BitmapImage(new Uri(@"/Resources/Icons/SetParam.png"));
                btnCommandSetSENameSys.Enabled = true;
            }
            #endregion
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a) 
        {
            return Result.Succeeded;
        }

        public static BitmapImage Convert(Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
