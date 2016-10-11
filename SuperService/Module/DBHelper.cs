using BitMobile.ClientModel3;
using BitMobile.DbEngine;
using System;
using System.Collections;
using Database = BitMobile.ClientModel3.Database;

namespace Test
{
    /// <summary>
    ///     Обеспечивает работу с базой данных приложения
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static partial class DBHelper
    {
        private const string EventStatusDoneName = "Done";
        private const string EventStatusCancelName = "Cancel";
        private const string EventStatusAppointed = "Appointed";
        private const string EventStatusInWork = "InWork";


        private static Database _db;

        public static string LastError => _db.LastError;
        public static DateTime LastSyncTime => _db.LastSyncTime;
        public static bool SuccessSync => _db.SuccessSync;

        public static void Init()
        {
            _db = new Database();
            if (_db.Exists) return;

            DConsole.WriteLine("Creating DB");
            _db.CreateFromModel();
        }

        public static void SaveEntity(DbEntity entity, bool doSync = true)
        {
            entity.Save();
            _db.Commit();
            if (doSync)
                SyncAsync();
        }

        public static void SaveEntities(IEnumerable entities, bool doSync = true)
        {
            foreach (DbEntity entity in entities)
            {
                entity.Save();
            }
            _db.Commit();
            if (doSync)
                SyncAsync();
        }

        public static void DeleteByRef(DbRef @ref, bool doSync = true)
        {
            _db.Delete(@ref);
            _db.Commit();
            if (doSync)
                SyncAsync();
        }

        public static object LoadEntity(string id)
        {
            return DbRef.FromString(id).GetObject();
        }

        public static void FullSyncAsync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSyncAsync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю полную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSyncAsync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformFullSyncAsync(Settings.Server, Settings.User, Settings.Password,
                    SyncHandler + resultEventHandler,
                    "Full");
            }
            catch (Exception)
            {
                SyncHandler("Full", new ResultEventArgs<bool>(false));
            }
        }

        public static void SyncAsync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(SyncAsync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю частичную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(SyncAsync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformSyncAsync(Settings.Server, Settings.User, Settings.Password,
                    SyncHandler + resultEventHandler,
                    "Partial");
            }
            catch (Exception)
            {
                SyncHandler("Partial", new ResultEventArgs<bool>(false));
            }
        }

        public static void Sync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(Sync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю частичную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(Sync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformSync(Settings.Server, Settings.User, Settings.Password,
                    SyncHandler + resultEventHandler,
                    "Partial");
            }
            catch (Exception)
            {
                SyncHandler("Partial", new ResultEventArgs<bool>(false));
            }
        }

        private static void SyncHandler(object state, ResultEventArgs<bool> resultEventArgs)
        {
            if (state.Equals("Full"))
            {
                Toast.MakeToast(Translator.Translate(resultEventArgs.Result ? "sync_success" : "sync_fail"));
            }
            else
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}"
                                   + Translator.Translate(resultEventArgs.Result ? "sync_success" : "sync_fail"));
                DConsole.WriteLine($"Последняя ошибка: {LastError}");
                DConsole.WriteLine($"Результат синхронизации в callback {resultEventArgs.Result}" +
                                   $"{Environment.NewLine}{nameof(SuccessSync)}: {SuccessSync}" +
                                   $"{Environment.NewLine}---------------");
#endif
            }
            if (!resultEventArgs.Result)
            {
#if DEBUG
                DConsole.WriteLine(Parameters.Splitter);
                DConsole.WriteLine($"Новые данные не пришли," +
                                   $"настройки не обновляем" +
                                   $" {nameof(resultEventArgs.Result)} = {resultEventArgs.Result}" +
                                   $"{Environment.NewLine}Last Error {_db.LastError}");
                DConsole.WriteLine(Parameters.Splitter);
#endif
                return;
            }
#if DEBUG
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine("Пришли новые настройки. Обновляем их");
            DConsole.WriteLine(Parameters.Splitter);
#endif
            GpsTracking.Stop();
            Settings.Init();
            GpsTracking.Start();
        }

        public static void FullSync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю полную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformFullSync(Settings.Server, Settings.User, Settings.Password,
                    SyncHandler + resultEventHandler,
                    "Full");
            }
            catch (Exception)
            {
                SyncHandler("Full", new ResultEventArgs<bool>(false));
            }
        }
    }
}